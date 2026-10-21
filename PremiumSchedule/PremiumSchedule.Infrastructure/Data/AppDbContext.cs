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
    }
}
