using ClaimManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimManagement.Infrastructure.Data;

public class ClaimDbContext : DbContext
{
    public ClaimDbContext(DbContextOptions<ClaimDbContext> options) : base(options)
    {
    }

    public DbSet<ClaimMaster> claim_master { get; set; }
    public DbSet<ClaimDocument> claim_documents { get; set; }
    public DbSet<ClaimHistory> claim_history { get; set; }
    public DbSet<SurveyAssignment> survey_assignment { get; set; }
    public DbSet<ClaimAssessment> claim_assessment { get; set; }
    public DbSet<InvestigationMaster> investigation { get; set; }
    public DbSet<InvestigationEvidence> investigation_evidence { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // claim number must be unique (BRD)
        modelBuilder.Entity<ClaimMaster>().HasIndex(c => c.claim_number).IsUnique();
        modelBuilder.Entity<ClaimMaster>().Property(c => c.estimated_loss).HasPrecision(15, 2);

        // links inside OUR database only (never to other services' databases)
        modelBuilder.Entity<ClaimMaster>().HasMany(c => c.claim_documents).WithOne().HasForeignKey(d => d.claim_id);
        modelBuilder.Entity<ClaimMaster>().HasMany(c => c.claim_history).WithOne().HasForeignKey(h => h.claim_id);

        modelBuilder.Entity<SurveyAssignment>().HasOne<ClaimMaster>().WithMany().HasForeignKey(s => s.claim_id);
        modelBuilder.Entity<SurveyAssignment>().HasIndex(s => s.claim_id).IsUnique();   // one survey per claim

        modelBuilder.Entity<ClaimAssessment>().HasOne<ClaimMaster>().WithMany().HasForeignKey(a => a.claim_id);
        modelBuilder.Entity<ClaimAssessment>().HasIndex(a => a.claim_id).IsUnique();    // one assessment per claim
        modelBuilder.Entity<ClaimAssessment>().Property(a => a.assessed_amount).HasPrecision(15, 2);
        modelBuilder.Entity<ClaimAssessment>().Property(a => a.deductible).HasPrecision(15, 2);
        modelBuilder.Entity<ClaimAssessment>().Property(a => a.depreciation).HasPrecision(15, 2);
        modelBuilder.Entity<ClaimAssessment>().Property(a => a.non_payable).HasPrecision(15, 2);
        modelBuilder.Entity<ClaimAssessment>().Property(a => a.net_payable).HasPrecision(15, 2);

        // investigation
        modelBuilder.Entity<InvestigationMaster>().HasOne<ClaimMaster>().WithMany().HasForeignKey(i => i.claim_id);
        modelBuilder.Entity<InvestigationMaster>().HasIndex(i => i.investigation_number).IsUnique();
        modelBuilder.Entity<InvestigationEvidence>().HasOne<InvestigationMaster>().WithMany().HasForeignKey(e => e.investigation_id);
    }
}