using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DiscountEngine.Application;
using DiscountEngine.Grpc.Interceptors;
using DiscountEngine.Grpc.Services;
using DiscountEngine.Grpc.Protos;
using DiscountEngine.Infrastructure.Data;
using DiscountEngine.Infrastructure;
using Grpc.Core;
using FluentAssertions;

namespace DiscountEngine.IntegrationTests
{
    [TestFixture]
    public class DiscountGrpcTests
    {
        private TestServer? _server;
        private GrpcChannel? _channel;
        private SqliteConnection? _connection;

        protected DiscountProtoService.DiscountProtoServiceClient Client { get; private set; }

        [OneTimeSetUp]
        public async Task GlobalSetupAsync()
        {
            SetupInMemoryDatabase();
            await StartTestServerAsync();
            CreateGrpcClient();
            await EnsureDatabaseCreatedAsync();
        }

        [Test]
        public async Task GetDiscount_ShouldReturnExpectedResult()
        {
            // Arrange
            const string existingCode = "DISC10";

            // Act
            var response = await Client.GetDiscountAsync(new GetDiscountRequest { Code = existingCode });

            // Assert
            response.Should().NotBeNull();
            response.Code.Should().Be(existingCode);
        }

        [Test]
        public void GetDiscount_NonExistentCode_ShouldThrowRpcException()
        {
            // Arrange
            const string nonExistentCode = "INVALID_CODE";

            // Act
            var ex = Assert.ThrowsAsync<RpcException>(async () =>
                await Client.GetDiscountAsync(new GetDiscountRequest { Code = nonExistentCode }));

            //Assert
            ex.Status.StatusCode.Should().Be(StatusCode.NotFound);
            ex.Status.Detail.Should().Be($"Discount with code '{nonExistentCode}' was not found.");
        }

        [Test]
        public void GetDiscount_EmptyCode_ShouldThrowRpcException()
        {
            // Act
            var ex = Assert.ThrowsAsync<RpcException>(async () =>
                await Client.GetDiscountAsync(new GetDiscountRequest { Code = "" }));

            //Assert
            ex.Status.StatusCode.Should().Be(StatusCode.NotFound);
            ex.Status.Detail.Should().Be($"Discount with code '' was not found.");
        }

        [Test]
        public async Task CreateDiscount_ShouldSucceed()
        {
            //Arrange
            var request = new CreateDiscountRequest
            {
                Code = "NEW10",
                Description = "10% New Year Discount",
                ProductCode = "ILS",
                Amount = 10
            };

            //Act
            var response = await Client.CreateDiscountAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.Code.Should().Be(request.Code);
            response.Description.Should().Be(request.Description);
            response.ProductCode.Should().Be(request.ProductCode);
            response.Amount.Should().Be(request.Amount);
        }

        [Test]
        public void CreateDiscount_WithDuplicateCode_ShouldThrowAlreadyExists()
        {
            // Arrange
            const string existingCode = "DISC10";
            var request = new CreateDiscountRequest
            {
                Code = existingCode,
                Description = "10% New Year Discount",
                ProductCode = "ILS",
                Amount = 10
            };

            // Act
            var ex = Assert.ThrowsAsync<RpcException>(async () =>
                await Client.CreateDiscountAsync(request));

            // Assert
            ex.Status.StatusCode.Should().Be(StatusCode.AlreadyExists);
            ex.Status.Detail.Should().Be($"A discount with code '{existingCode}' already exists.");
        }

        [Test]
        public async Task UpdateDiscount_ShouldSucceed()
        {
            //Arrange 
            var updateRequest = new UpdateDiscountRequest
            {
                Id = -1,
                Code = "DISC10",
                ProductCode = "P1001",
                Amount = 20,
                Description = "10% off on product P1001"
            };

            //Act
            var response = await Client.UpdateDiscountAsync(updateRequest);

            //Assert
            response.Should().NotBeNull();
            response.Code.Should().Be(updateRequest.Code);
            response.Id.Should().Be(updateRequest.Id);
            response.Description.Should().Be(updateRequest.Description);
            response.ProductCode.Should().Be(updateRequest.ProductCode);
            response.Amount.Should().Be(updateRequest.Amount);
        }

        [Test]
        public void UpdateDiscount_NonExistentCode_ShouldThrowNotFound()
        {
            //Arrange
            var request = new UpdateDiscountRequest
            {
                Id = 10,
                Code = "INVALID",
                Description = "Won't update",
                Amount = 12.0
            };

            // Act
            var ex = Assert.ThrowsAsync<RpcException>(async () =>
                await Client.UpdateDiscountAsync(request));

            //Assert
            ex.Status.StatusCode.Should().Be(StatusCode.NotFound);
            ex.Status.Detail.Should().Be($"Discount with ID '{request.Id}' was not found.");
        }

        [Test]
        public async Task DeleteDiscount_ShouldSucceed()
        {
            // Arrange
            var discount = await Client.CreateDiscountAsync(new CreateDiscountRequest
            {
                Code = "DEL10",
                Description = "To be deleted",
                Amount = 5.0,
                ProductCode = "ILS"
            });

            // Act
            var response = await Client.DeleteDiscountAsync(new DeleteDiscountRequest { Code = "DEL10" });

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
        }

        [Test]
        public void DeleteDiscount_NonExistentCode_ShouldReturnFalse()
        {
            // Arrange
            const string nonExistentCode = "INVALID_CODE";
            var request = new DeleteDiscountRequest
            {
                Code = nonExistentCode,
            };

            // Act
            var ex = Assert.ThrowsAsync<RpcException>(async () =>
                await Client.DeleteDiscountAsync(request));

            // Assert
            ex.Status.StatusCode.Should().Be(StatusCode.NotFound);
            ex.Status.Detail.Should().Be($"Discount with code '{nonExistentCode}' was not found.");
        }

        private void SetupInMemoryDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        private async Task StartTestServerAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection([])
                .Build();

            _server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(config => config.AddConfiguration(configuration))
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DiscountDbContext>(options => options.UseSqlite(_connection!));

                    services.AddScoped<ExceptionHandlingInterceptor>();
                    services.AddScoped<CorrelationIdInterceptor>();

                    services.AddGrpc(options =>
                    {
                        options.Interceptors.Add<CorrelationIdInterceptor>();
                        options.Interceptors.Add<ExceptionHandlingInterceptor>();
                    });

                    services.AddInfrastructure(configuration);
                    services.AddApplication();
                    services.AddScoped<DiscountGrpcService>();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<DiscountGrpcService>();
                    });
                }));

            await Task.CompletedTask;
        }

        private void CreateGrpcClient()
        {
            var client = _server!.CreateClient();
            _channel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions { HttpClient = client });
            Client = new DiscountProtoService.DiscountProtoServiceClient(_channel);
        }

        private async Task EnsureDatabaseCreatedAsync()
        {
            using var scope = _server!.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DiscountDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardownAsync()
        {
            if (_channel != null)
            {
                await _channel.ShutdownAsync();
                _channel.Dispose();
                _channel = null;
            }

            _server?.Dispose();
            _server = null;

            _connection?.Dispose();
            _connection = null;
        }
    }
}
