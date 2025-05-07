using DiscountEngine.Application;
using DiscountEngine.Grpc.Interceptors;
using DiscountEngine.Grpc.Services;
using DiscountEngine.Infrastructure;
using DiscountEngine.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using System.Text.Json;

namespace DiscountEngine.Grpc;

public class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .CreateBootstrapLogger();
        try
        {
            Log.Information("Engine starting up...");

            var builder = WebApplication.CreateBuilder(args);
            // Configure Serilog with full settings
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console();
            });

            // Add services to the container.
            builder.Services.AddScoped<ExceptionHandlingInterceptor>();
            builder.Services.AddScoped<CorrelationIdInterceptor>();
            builder.Services.AddGrpc(options =>
            {
                options.Interceptors.Add<CorrelationIdInterceptor>();
                options.Interceptors.Add<ExceptionHandlingInterceptor>();
            });
            builder.Services.AddInfrastructure(builder.Configuration!);
            builder.Services.AddApplication();
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<DiscountDbContext>("SQLite Database");

            var app = builder.Build();

            app.ApplyMigrations();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<DiscountGrpcService>();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            exception = e.Value.Exception?.Message,
                            duration = e.Value.Duration.ToString()
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            });

            app.Run();



        }
        catch (Exception exception)
        {

            Log.Fatal(exception, "Application startup failed!");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }



    }
}