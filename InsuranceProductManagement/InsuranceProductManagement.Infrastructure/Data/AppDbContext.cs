using Microsoft.EntityFrameworkCore;
using InsuranceProductManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace InsuranceProductManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<InsuranceProduct> InsuranceProducts { get; set; }

        public DbSet<CoverageMaster> CoverageMasters { get; set; }

        public DbSet<RiderMaster> RiderMasters { get; set; }

        public DbSet<BenefitMaster> BenefitMasters { get; set; }

        public DbSet<PremiumFrequency> PremiumFrequencies { get; set; }

        public DbSet<ProductCommission> ProductCommissions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            

          
            builder.Entity<InsuranceProduct>()
                .HasOne<ProductCategory>()
                .WithMany()
                .HasForeignKey(p => p.category_id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CoverageMaster>()
                .HasOne<InsuranceProduct>()
                .WithMany()
                .HasForeignKey(c => c.product_id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RiderMaster>()
                .HasOne<InsuranceProduct>()
                .WithMany()
                .HasForeignKey(r => r.product_id)
                .OnDelete(DeleteBehavior.Restrict);

          
            builder.Entity<BenefitMaster>()
                .HasOne<InsuranceProduct>()
                .WithMany()
                .HasForeignKey(b => b.product_id)
                .OnDelete(DeleteBehavior.Restrict);

           
            builder.Entity<ProductCommission>()
                .HasOne<InsuranceProduct>()
                .WithMany()
                .HasForeignKey(pc => pc.product_id)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<ProductCategory>()
                .ToTable("product_category");

            builder.Entity<InsuranceProduct>()
                .ToTable("insurance_product");

            builder.Entity<CoverageMaster>()
                .ToTable("coverage_master");

            builder.Entity<RiderMaster>()
                .ToTable("rider_master");

            builder.Entity<BenefitMaster>()
                .ToTable("benefit_master");

            builder.Entity<PremiumFrequency>()
                .ToTable("premium_frequency");

            builder.Entity<ProductCommission>()
                .ToTable("product_commission");

        }


    }
}
