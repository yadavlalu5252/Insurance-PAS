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
    }
}
