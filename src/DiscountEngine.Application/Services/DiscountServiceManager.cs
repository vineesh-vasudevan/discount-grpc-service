using DiscountEngine.Domain.Repositories;
using DiscountEngine.Domain.Exceptions;
using DiscountEngine.Domain.Entities;
using DiscountEngine.Domain.Services;

namespace DiscountEngine.Application.Services
{
    public class DiscountServiceManager(IDiscountRepository discountRepository) : IDiscountService
    {
        public async Task<Discount> GetDiscount(string code, CancellationToken cancellationToken)
        {
            var discount = await discountRepository.GetByCodeAsync(code, cancellationToken);
            if (discount.HasNoValue)
            {
                throw new DiscountNotFoundException(code);
            }
            return discount.Value;
        }

        public async Task<Discount> CreateAsync(Discount discount, CancellationToken cancellationToken)
        {
            var existing = await discountRepository.GetByCodeAsync(discount.Code, cancellationToken);
            if (existing.HasValue)
                throw new DiscountAlreadyExistsException(discount.Code);

            await discountRepository.AddAsync(discount, cancellationToken);
            return discount;
        }

        public async Task<Discount> UpdateAsync(Discount discount, CancellationToken cancellationToken)
        {
            var existingDiscount = await discountRepository.GetByIdAsync(discount.Id, cancellationToken);
            if (!existingDiscount.HasValue)
                throw new DiscountNotFoundException(discount.Id);

            existingDiscount.Value.Code = discount.Code;
            existingDiscount.Value.ProductCode = discount.ProductCode;
            existingDiscount.Value.Amount = discount.Amount;
            existingDiscount.Value.Description = discount.Description;

            await discountRepository.UpdateAsync(existingDiscount.Value, cancellationToken);
            return discount;
        }

        public async Task<bool> DeleteByCodeAsync(string code, CancellationToken cancellationToken)
        {
            var existing = await discountRepository.GetByCodeAsync(code, cancellationToken);

            if (existing.HasNoValue)
            {
                throw new DiscountNotFoundException(code);
            }

            await discountRepository.DeleteAsync(existing.Value, cancellationToken);
            return true;
        }
    }
}
