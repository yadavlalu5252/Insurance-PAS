using ClaimSettlement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimSettlement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        public DbSet<Settlement> ClaimSettlements { get; set; } 
    }
}