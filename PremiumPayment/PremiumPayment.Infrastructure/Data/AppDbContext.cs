using Microsoft.EntityFrameworkCore;
using PremiumPayment.Domain.Entities;
using PremiumPaymentEntity = PremiumPayment.Domain.Entities.PremiumPayments;

namespace PremiumPayment.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PremiumPaymentEntity> PremiumPayments { get; set; }
        public DbSet<PremiumReminder> PremiumReminders { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }

        public DbSet<PremiumRefund> PremiumRefunds { get; set; }





        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<PremiumPaymentEntity>(entity =>
        //    {
        //        entity.ToTable("premium_payment");

        //        entity.HasKey(x => x.payment_id);

        //        entity.Property(x => x.payment_id)
        //            .ValueGeneratedOnAdd();

        //        entity.Property(x => x.schedule_id)
        //            .IsRequired();

        //        entity.Property(x => x.policy_id)
        //            .IsRequired();

        //        entity.Property(x => x.payment_date)
        //            .IsRequired();

        //        entity.Property(x => x.amount_paid)
        //            .HasPrecision(15, 2)
        //            .IsRequired();

        //        entity.Property(x => x.payment_mode)
        //            .HasMaxLength(50)
        //            .IsRequired();

        //        entity.Property(x => x.transaction_reference)
        //            .HasMaxLength(100);

        //        entity.Property(x => x.payment_status)
        //            .HasMaxLength(30)
        //            .IsRequired();

        //        entity.HasIndex(x => x.transaction_reference)
        //            .IsUnique()
        //            .HasFilter("[TransactionReference] IS NOT NULL");
        //    });
        //}
    }
}