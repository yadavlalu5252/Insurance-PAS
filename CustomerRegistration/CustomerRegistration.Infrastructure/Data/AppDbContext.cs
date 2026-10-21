using CustomerRegistration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerRegistration.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        public DbSet<Dummy> dms { get; set; }

        public DbSet<Customer> customers { get; set; }
        public DbSet<CustomerAddress> customer_addresses { get; set; }
        public DbSet<CustomerContact> customer_contacts { get; set; }
        public DbSet<Nominee> nominees { get; set; }
        public DbSet<CustomerDocument> customer_documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.customer_id);

                entity.HasIndex(e => e.customer_code).IsUnique();
                entity.HasIndex(e => e.mobile_number).IsUnique();
                entity.HasIndex(e => e.email).IsUnique();

                entity.Property(e => e.customer_status)
                      .HasDefaultValue("Active");

                entity.Property(e => e.created_at)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.updated_at)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // Customer -> Addresses (one-to-many)
            modelBuilder.Entity<CustomerAddress>(entity =>
            {
                entity.HasKey(e => e.address_id);

                entity.HasOne(e => e.customer)
                      .WithMany(e => e.addresses)
                      .HasForeignKey(e => e.customer_id)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Customer -> Contact (one-to-one)
            modelBuilder.Entity<CustomerContact>(entity =>
            {
                entity.HasKey(e => e.contact_id);

                entity.HasIndex(e => e.customer_id)
                      .IsUnique();

                entity.HasOne(e => e.customer)
                      .WithOne(e => e.contact)
                      .HasForeignKey<CustomerContact>(e => e.customer_id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.preferred_contact_mode)
                      .HasDefaultValue("Mobile");
            });

            // Customer -> Nominees (one-to-many)
            modelBuilder.Entity<Nominee>(entity =>
            {
                entity.HasKey(e => e.nominee_id);

                entity.Property(e => e.share_percentage)
                      .HasPrecision(5, 2);

                entity.HasOne(e => e.customer)
                      .WithMany(e => e.nominees)
                      .HasForeignKey(e => e.customer_id)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Customer -> Documents (one-to-many)
            modelBuilder.Entity<CustomerDocument>(entity =>
            {
                entity.HasKey(e => e.document_id);

                entity.Property(e => e.verification_status)
                      .HasDefaultValue("Pending");

                entity.Property(e => e.uploaded_date)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.customer)
                      .WithMany(e => e.documents)
                      .HasForeignKey(e => e.customer_id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}