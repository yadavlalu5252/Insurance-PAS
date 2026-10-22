
using Microsoft.EntityFrameworkCore;
using Underwriting.Domain.Entities;

namespace Underwriting.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<underwriting_case> underwriting_case { get; set; }
        public DbSet<medical_underwriting> medical_underwriting { get; set; }
        public DbSet<financial_underwriting> financial_underwriting { get; set; }
        public DbSet<risk_score> risk_score { get; set; }
        public DbSet<underwriting_decision> underwriting_decision { get; set; }
        public DbSet<underwriting_audit_log> underwriting_audit_log { get; set; }
    }
}
