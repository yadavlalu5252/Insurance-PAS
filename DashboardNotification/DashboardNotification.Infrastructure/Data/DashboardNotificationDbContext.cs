
using DashboardNotification.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DashboardNotification.Infrastructure.Data
{
    public class DashboardNotificationDbContext : DbContext
    {
        public DashboardNotificationDbContext(DbContextOptions<DashboardNotificationDbContext> options): base(options)
        {

        }

        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.ToTable("notification_template");

                entity.HasKey(x => x.template_id);

                entity.Property(x => x.template_id)
                    .HasColumnName("template_id")
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.template_name)
                    .HasColumnName("template_name")
                    .HasColumnType("varchar(150)");

                entity.Property(x => x.notification_type)
                    .HasColumnName("notification_type")
                    .HasColumnType("varchar(20)");

                entity.Property(x => x.subject)
                    .HasColumnName("subject")
                    .HasColumnType("varchar(255)");

                entity.Property(x => x.body)
                    .HasColumnName("body")
                    .HasColumnType("varchar(max)");

                entity.Property(x => x.version_no)
                    .HasColumnName("version_no")
                    .HasColumnType("int");

                entity.Property(x => x.status)
                    .HasColumnName("status")
                    .HasColumnType("varchar(10)");
            });
        }
    }
}
