using DiscountEngine.Domain.Entities;

namespace DiscountEngine.Domain.Services
{
    public interface IDiscountService
    {
        Task<Discount> GetDiscount(string code, CancellationToken cancellationToken);

        Task<Discount> CreateAsync(Discount discount, CancellationToken cancellationToken);

        Task<Discount> UpdateAsync(Discount discount, CancellationToken cancellationToken);

        Task<bool> DeleteByCodeAsync(string code, CancellationToken cancellationToken);
    }
}
