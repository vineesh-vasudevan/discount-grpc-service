﻿using CSharpFunctionalExtensions;
using DiscountEngine.Domain.Entities;
using DiscountEngine.Domain.Repositories;
using DiscountEngine.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscountEngine.Infrastructure.Repositories
{
    public class DiscountRepository(DiscountDbContext dbContext) : IDiscountRepository
    {
        public async Task<Maybe<Discount>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var discount = await dbContext.Discounts.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            return Maybe.From(discount);
        }

        public async Task<Maybe<Discount>> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            var discount = await dbContext.Discounts.FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
            return Maybe.From(discount);
        }

        public async Task AddAsync(Discount discount, CancellationToken cancellationToken)
        {
            await dbContext.Discounts.AddAsync(discount, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Discount discount, CancellationToken cancellationToken)
        {
            dbContext.Discounts.Update(discount);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Discount discount, CancellationToken cancellationToken)
        {
            dbContext.Discounts.Remove(discount);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
