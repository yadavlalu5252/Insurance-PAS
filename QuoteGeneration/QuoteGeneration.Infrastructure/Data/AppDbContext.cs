using InsuranceProductManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace QuoteGeneration.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Quote> Quotes { get; set; }

        public DbSet<PremiumCalculation> PremiumCalculations { get; set; }

        public DbSet<RiskAssessment> RiskAssessments { get; set; }

        public DbSet<QuoteRider> QuoteRiders { get; set; }

        public DbSet<DiscountMaster> DiscountMasters { get; set; }

     
        public DbSet<QuoteHistory> QuoteHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {


            builder.Entity<PremiumCalculation>()
                .HasOne<Quote>()
                .WithMany()
                .HasForeignKey(p => p.quote_id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RiskAssessment>()
                .HasOne<Quote>()
                .WithMany()
                .HasForeignKey(r => r.quote_id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuoteRider>()
                .HasOne<Quote>()
                .WithMany()
                .HasForeignKey(r => r.quote_id)
                .OnDelete(DeleteBehavior.Restrict);

           
            builder.Entity<QuoteHistory>()
                .HasOne<Quote>()
                .WithMany()
                .HasForeignKey(h => h.quote_id)
                .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<Quote>()
                .ToTable("quote");

            builder.Entity<PremiumCalculation>()
                .ToTable("premium_calculation");

            builder.Entity<RiskAssessment>()
                .ToTable("risk_assessment");

            builder.Entity<QuoteRider>()
                .ToTable("quote_rider");

            builder.Entity<DiscountMaster>()
                .ToTable("discount_master");

            builder.Entity<QuoteHistory>()
                .ToTable("quote_history");

        }

    }
    }
