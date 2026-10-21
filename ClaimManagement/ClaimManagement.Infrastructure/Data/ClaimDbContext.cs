using ClaimManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace ClaimManagement.Infrastructure.Data
{
    public class ClaimDbContext : DbContext
    {
        public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options)
        {
        }

        public DbSet<ClaimMaster> claim_master { get; set; }
        public DbSet<ClaimDocument> claim_documents { get; set; }
        public DbSet<ClaimHistory> claim_history { get; set; }
        public DbSet<SurveyAssignment> survey_assignment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // claim number must be unique (BRD rule)
            modelBuilder.Entity<ClaimMaster>()
                .HasIndex(c => c.claim_number)
                .IsUnique();

            modelBuilder.Entity<ClaimMaster>()
                .Property(c => c.estimated_loss)
                .HasPrecision(15, 2);
        }
    }
}
