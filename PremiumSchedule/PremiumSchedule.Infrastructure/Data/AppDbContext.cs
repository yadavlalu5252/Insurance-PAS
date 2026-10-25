using Microsoft.EntityFrameworkCore;
using PremiumSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) 
        {
            
        }
        public DbSet<PremiumSchedules> PremiumSchedule { get; set; }
        public DbSet<PremiumPayments> PremiumPayments { get; set; }
        public DbSet<PremiumReminder> PremiumReminders { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<PremiumRefund> PremiumRefunds { get; set; }

    }
}
