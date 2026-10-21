using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}