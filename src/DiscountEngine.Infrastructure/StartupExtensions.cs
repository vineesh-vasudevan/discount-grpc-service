using DiscountEngine.Domain.Repositories;
using DiscountEngine.Infrastructure.Data;
using DiscountEngine.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountEngine.Infrastructure
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnection = configuration.GetConnectionString("DiscountDb");
            services.AddDbContext<DiscountDbContext>(opts =>
                opts.UseSqlite(dbConnection));

            services.AddScoped<IDiscountRepository, DiscountRepository>();
            return services;
        }
    }
}
