using Microsoft.EntityFrameworkCore;

namespace DiscountEngine.Infrastructure.Data
{
    public class DiscountDbContext(DbContextOptions<DiscountDbContext> options) : DbContext(options)
    {
        public DbSet<Domain.Entities.Discount> Discounts { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountDbContext).Assembly);
        }
    }
}
