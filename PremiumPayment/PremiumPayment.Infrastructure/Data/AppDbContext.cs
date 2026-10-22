using Microsoft.EntityFrameworkCore;
using PremiumPaymentEntity = Premium.Domain.Entities.PremiumPayment;

namespace PremiumPayment.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PremiumPaymentEntity> PremiumPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PremiumPaymentEntity>(entity =>
            {
                entity.ToTable("premium_payment");

                entity.HasKey(x => x.PaymentId);

                entity.Property(x => x.PaymentId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.ScheduleId)
                    .IsRequired();

                entity.Property(x => x.PolicyId)
                    .IsRequired();

                entity.Property(x => x.PaymentDate)
                    .IsRequired();

                entity.Property(x => x.AmountPaid)
                    .HasPrecision(15, 2)
                    .IsRequired();

                entity.Property(x => x.PaymentMode)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.TransactionReference)
                    .HasMaxLength(100);

                entity.Property(x => x.PaymentStatus)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasIndex(x => x.TransactionReference)
                    .IsUnique()
                    .HasFilter("[TransactionReference] IS NOT NULL");
            });
        }
    }
}