
using Microsoft.EntityFrameworkCore;
using ReportsDashboard.Domain.Entities;

namespace ReportsDashboard.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<report_master> report_master { get; set; }

        public DbSet<report_schedule> report_schedule { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<report_master>().HasData(
                new report_master
                {
                    report_id = 1,
                    report_name = "Customer Report",
                    report_type = "Customer",
                    description = "Customer-related reports",
                    created_date = new DateTime(2026, 1, 1)
                },
                new report_master
                {
                    report_id = 2,
                    report_name = "Policy Report",
                    report_type = "Policy",
                    description = "Policy-related reports",
                    created_date = new DateTime(2026, 1, 1)
                },
                new report_master
                {
                    report_id = 3,
                    report_name = "Premium Report",
                    report_type = "Premium",
                    description = "Premium collection reports",
                    created_date = new DateTime(2026, 1, 1)
                },
                new report_master
                {
                    report_id = 4,
                    report_name = "Claims Report",
                    report_type = "Claims",
                    description = "Claims-related reports",
                    created_date = new DateTime(2026, 1, 1)
                },
                new report_master
                {
                    report_id = 5,
                    report_name = "Underwriting Report",
                    report_type = "Underwriting",
                    description = "Underwriting-related reports",
                    created_date = new DateTime(2026, 1, 1)
                },
                new report_master
                {
                    report_id = 6,
                    report_name = "Agent Report",
                    report_type = "Agent",
                    description = "Agent performance reports",
                    created_date = new DateTime(2026, 1, 1)
                },
                new report_master
                {
                    report_id = 7,
                    report_name = "Financial Report",
                    report_type = "Financial",
                    description = "Financial reports",
                    created_date = new DateTime(2026, 1, 1)
                });

            modelBuilder.Entity<report_schedule>()
                .HasOne(s => s.report)
                .WithMany(r => r.schedules)
                .HasForeignKey(s => s.report_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
