
using Microsoft.EntityFrameworkCore;
using PolicyIssuance.Domain.Entities;

namespace PolicyIssuance.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PolicyEntity> Policies { get; set; }

        public DbSet<PolicyCoverage> PolicyCoverages { get; set; }

        public DbSet<PolicyRider> PolicyRiders { get; set; }

        public DbSet<PolicyDocument> PolicyDocuments { get; set; }

        public DbSet<PolicyNominee> PolicyNominees { get; set; }

        public DbSet<PolicyEndorsement> PolicyEndorsements { get; set; }

        public DbSet<PolicyHistory> PolicyHistories { get; set; }

        public DbSet<PolicyClosure> PolicyClosures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PolicyEntity>()
                .Property(p => p.premium_amount)
                .HasPrecision(15, 2);

            modelBuilder.Entity<PolicyCoverage>()
                .Property(p => p.sum_insured)
                .HasPrecision(15, 2);

            modelBuilder.Entity<PolicyRider>()
                .Property(p => p.premium_amount)
                .HasPrecision(15, 2);

            modelBuilder.Entity<PolicyNominee>()
                .Property(p => p.share_percentage)
                .HasPrecision(5, 2);
        }
    }
}
