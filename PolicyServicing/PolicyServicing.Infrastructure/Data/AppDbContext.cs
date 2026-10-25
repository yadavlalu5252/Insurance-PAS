using Microsoft.EntityFrameworkCore;
using PolicyServicing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Infrastructure.Data
{
    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        public DbSet<nominee_details> nominee_details { get; set; }

        public DbSet<policy_endorsement> policy_endorsements { get; set; }

        public DbSet<policy_renewal> policy_renewal { get; set; }

        public DbSet<PolicyCancellation> PolicyCancellations { get; set; }

        public DbSet<service_request> service_request { get; set; }

        public DbSet<service_request_history> service_request_history { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<policy_endorsement>()
                .HasOne(pe => pe.service_Request)
                .WithMany(sr => sr.policy_endorsements)
                .HasForeignKey(pe => pe.request_id)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<service_request>()
        .Property(s => s.request_id)
        .ValueGeneratedOnAdd();
        }



    }
}
