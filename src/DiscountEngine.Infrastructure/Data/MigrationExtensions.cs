using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscountEngine.Infrastructure.Data
{
    public static class MigrationExtensions
    {
        public static IHost ApplyMigrations(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DiscountDbContext>();
            dbContext.Database.Migrate();
            return host;
        }
    }
}
