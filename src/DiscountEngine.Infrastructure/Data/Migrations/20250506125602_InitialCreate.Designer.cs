﻿// <auto-generated />
using DiscountEngine.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DiscountEngine.Infrastructure.Data.Migrations
{
    [DbContext(typeof(DiscountDbContext))]
    [Migration("20250506125602_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            modelBuilder.Entity("DiscountEngine.Domain.Entities.Discount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Amount")
                        .HasColumnType("REAL");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProductCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Discounts");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Amount = 10.0,
                            Code = "DISC10",
                            Description = "10% off on product P1001",
                            ProductCode = "P1001"
                        },
                        new
                        {
                            Id = -2,
                            Amount = 20.0,
                            Code = "DISC20",
                            Description = "20% off on product P2002",
                            ProductCode = "P2002"
                        },
                        new
                        {
                            Id = -3,
                            Amount = 30.0,
                            Code = "DISC30",
                            Description = "30% off on product P3003",
                            ProductCode = "P3003"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
