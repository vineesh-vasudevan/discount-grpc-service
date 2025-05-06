using DiscountEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscountEngine.Infrastructure.Data
{
    public class DiscountDbContext(DbContextOptions<DiscountDbContext> options) : DbContext(options)
    {
        public DbSet<Discount> Discounts { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountDbContext).Assembly);
        }
    }
}
