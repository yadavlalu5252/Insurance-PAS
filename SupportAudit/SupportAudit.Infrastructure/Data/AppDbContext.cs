using Microsoft.EntityFrameworkCore;
using SupportAudit.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportAudit.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    }
}
