using CSharpFunctionalExtensions;
using DiscountEngine.Domain.Entities;

namespace DiscountEngine.Domain.Repositories
{
    public interface IDiscountRepository
    {
        Task<Maybe<Discount>> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<Maybe<Discount>> GetByCodeAsync(string code, CancellationToken cancellationToken);

        Task AddAsync(Discount discount, CancellationToken cancellationToken);

        Task UpdateAsync(Discount discount, CancellationToken cancellationToken);

        Task DeleteAsync(Discount discount, CancellationToken cancellationToken);
    }
}
