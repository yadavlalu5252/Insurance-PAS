using InvestigationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestigationService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<InvestigationService.Domain.Entities.Investigation> Investigations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InvestigationService.Domain.Entities.Investigation>(entity =>
            {
                entity.ToTable("investigation");

                entity.HasKey(x => x.InvestigationId);

                entity.Property(x => x.InvestigationId)
                    .HasColumnName("investigation_id");

                entity.Property(x => x.ClaimId)
                    .HasColumnName("claim_id")
                    .IsRequired();

                entity.Property(x => x.InvestigatorId)
                    .HasColumnName("investigator_id")
                    .IsRequired();

                entity.Property(x => x.InvestigationReason)
                    .HasColumnName("investigation_reason")
                    .IsRequired();

                entity.Property(x => x.ReportPath)
                    .HasColumnName("report_path")
                    .HasMaxLength(500);

                entity.Property(x => x.InvestigationStatus)
                    .HasColumnName("investigation_status")
                    .HasConversion<string>()
                    .IsRequired();
            });
        }
    }
}