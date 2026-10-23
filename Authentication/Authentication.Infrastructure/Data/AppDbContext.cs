
using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> user_master { get; set; }

        public DbSet<Role> role_master { get; set; }

        public DbSet<Permission> permission_master { get; set; }

        public DbSet<RolePermission> role_permission { get; set; }

        public DbSet<RefreshToken> refresh_tokens { get; set; }

        public DbSet<PasswordResetToken> password_reset_tokens { get; set; }

        public DbSet<TwoFactorLoginChallenge> two_factor_login_challenges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TwoFactorLoginChallenge>()
                .HasIndex(c => c.challenge_token_hash)
                .IsUnique();

            modelBuilder.Entity<TwoFactorLoginChallenge>()
                .HasIndex(c => c.user_id);
        }
    }
}
