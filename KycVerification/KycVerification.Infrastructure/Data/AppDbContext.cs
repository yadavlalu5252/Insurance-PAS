using Microsoft.EntityFrameworkCore;
using KycVerification.Domain.Entities;

namespace KycVerification.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerKyc> CustomerKycs { get; set; }

        public DbSet<KycDocument> KycDocuments { get; set; }

        public DbSet<CkycDetails> CkycDetails { get; set; }

        public DbSet<KycVerificationLog> KycVerificationLogs { get; set; }

        public DbSet<DocumentTypeMaster> DocumentTypeMasters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerKyc>()
                .ToTable("customer_kyc");

            modelBuilder.Entity<KycDocument>()
                .ToTable("kyc_document");

            modelBuilder.Entity<CkycDetails>()
                .ToTable("ckyc_details");

            modelBuilder.Entity<KycVerificationLog>()
                .ToTable("kyc_verification_log");

            modelBuilder.Entity<DocumentTypeMaster>()
                .ToTable("document_type_master");
        }
    }
}