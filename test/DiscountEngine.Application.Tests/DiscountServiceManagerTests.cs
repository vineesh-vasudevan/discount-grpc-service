using CSharpFunctionalExtensions;
using DiscountEngine.Application.Services;
using DiscountEngine.Domain.Entities;
using DiscountEngine.Domain.Exceptions;
using DiscountEngine.Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace DiscountEngine.Application.Tests
{
    [TestFixture]
    public class DiscountServiceManagerTests
    {
        private IDiscountRepository _discountRepository = null!;
        private DiscountServiceManager _sut = null!;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        [SetUp]
        public void Setup()
        {
            _discountRepository = Substitute.For<IDiscountRepository>();
            _sut = new DiscountServiceManager(_discountRepository);
        }

        [Test]
        public async Task GetDiscount_ShouldReturnDiscount_WhenFound()
        {
            //Arrange
            var discount = new Discount { Id = 1, Code = "SAVE10" };
            _discountRepository
                .GetByCodeAsync("SAVE10", _cancellationToken)
                .Returns(discount);

            //Act
            var result = await _sut.GetDiscount("SAVE10", _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(discount);
        }

        [Test]
        public async Task GetDiscount_ShouldThrow_WhenNotFound()
        {
            //Arrange
            const string code = "SAVE10";

            _discountRepository
                .GetByCodeAsync(code, _cancellationToken)
                .Returns(Maybe<Discount>.None);

            //Act
            Func<Task> act = async () => await _sut.GetDiscount(code, _cancellationToken);

            //Assert
            await act.Should().ThrowAsync<DiscountNotFoundException>();
        }

        [Test]
        public async Task CreateAsync_ShouldAddDiscount_WhenNotExists()
        {
            //Act
            var discount = new Discount { Code = "SAVE10" };
            _discountRepository
                .GetByCodeAsync("SAVE10", _cancellationToken)
                .Returns(Maybe<Discount>.None);

            //Assert
            var result = await _sut.CreateAsync(discount, _cancellationToken);

            //Arrange
            result.Should().Be(discount);
            await _discountRepository
                .Received(1)
                .AddAsync(discount, _cancellationToken);
        }

        [Test]
        public async Task CreateAsync_ShouldThrow_WhenAlreadyExists()
        {
            //Arrange
            var discount = new Discount { Code = "SAVE10" };
            _discountRepository
                .GetByCodeAsync("SAVE10", _cancellationToken)
                .Returns(discount);

            //Act
            Func<Task> act = async () => await _sut.CreateAsync(discount, _cancellationToken);

            //Assert
            await act.Should().ThrowAsync<DiscountAlreadyExistsException>();
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdate_WhenExists()
        {
            //Arrange
            var existing = new Discount { Id = 1, Code = "OLD" };
            var updated = new Discount
            {
                Id = 1,
                Code = "NEW",
                ProductCode = "P123",
                Amount = 50,
                Description = "Updated"
            };

            _discountRepository.
                GetByIdAsync(1, _cancellationToken)
                .Returns(existing);

            //Act
            var result = await _sut.UpdateAsync(updated, _cancellationToken);

            //Assert
            result.Should().Be(updated);
            existing.Code.Should().Be("NEW");
            existing.ProductCode.Should().Be("P123");

            await _discountRepository
                .Received(1)
                .UpdateAsync(existing, _cancellationToken);
        }

        [Test]
        public async Task UpdateAsync_ShouldThrow_WhenNotFound()
        {
            //Arrange
            var discount = new Discount { Id = 1 };
            _discountRepository
                .GetByIdAsync(1, _cancellationToken)
                .Returns(Maybe<Discount>.None);

            //Act
            Func<Task> act = async () => await _sut.UpdateAsync(discount, _cancellationToken);

            //Assert
            await act.Should().ThrowAsync<DiscountNotFoundException>();
        }

        [Test]
        public async Task DeleteByCodeAsync_ShouldDelete_WhenExists()
        {
            //Arrange
            var discount = new Discount { Code = "SAVE10" };
            _discountRepository
                .GetByCodeAsync("SAVE10", _cancellationToken)
                .Returns(discount);

            //Act
            var result = await _sut.DeleteByCodeAsync("SAVE10", _cancellationToken);

            //Assert
            result.Should().BeTrue();
            await _discountRepository
                .Received(1)
                .DeleteAsync(discount, _cancellationToken);
        }

        [Test]
        public async Task DeleteByCodeAsync_ShouldThrow_WhenNotFound()
        {
            //Arrange
            _discountRepository
                .GetByCodeAsync("SAVE10", _cancellationToken)
                .Returns(Maybe<Discount>.None);

            //Act
            Func<Task> act = async () => await _sut.DeleteByCodeAsync("SAVE10", _cancellationToken);

            //Assert
            await act.Should().ThrowAsync<DiscountNotFoundException>();
        }

    }
}
