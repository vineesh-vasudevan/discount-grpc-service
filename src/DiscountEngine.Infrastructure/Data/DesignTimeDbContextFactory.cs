using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DiscountEngine.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DiscountDbContext>
    {
        public DiscountDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "DiscountEngine.Grpc");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(basePath)
               .AddJsonFile("appsettings.json")
               .Build();


            var dbConnection = configuration.GetConnectionString("DiscountDb");
            var optionsBuilder = new DbContextOptionsBuilder<DiscountDbContext>();
            optionsBuilder.UseSqlite(dbConnection);

            return new DiscountDbContext(optionsBuilder.Options);
        }
    }
}
