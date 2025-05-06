using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DiscountEngine.Domain.Entities;

namespace DiscountEngine.Infrastructure.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            builder.Property(d => d.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(d => d.ProductCode)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(d => d.Description)
                   .HasMaxLength(250);

            builder.HasIndex(d => d.Code).IsUnique();

            builder.HasData(
                new Discount { Id = -1, Code = "DISC10", ProductCode = "P1001", Amount = 10, Description = "10% off on product P1001" },
                new Discount { Id = -2, Code = "DISC20", ProductCode = "P2002", Amount = 20, Description = "20% off on product P2002" },
                new Discount { Id = -3, Code = "DISC30", ProductCode = "P3003", Amount = 30, Description = "30% off on product P3003" }
            );
        }
    }
}
