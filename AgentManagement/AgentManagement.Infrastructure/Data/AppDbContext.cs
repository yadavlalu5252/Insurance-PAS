
using Microsoft.EntityFrameworkCore;
using AgentManagement.Domain.Entities;

namespace AgentManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Agent> Agents { get; set; }

        public DbSet<AgentCustomer> AgentCustomers { get; set; }

        public DbSet<AgentPolicy> AgentPolicies { get; set; }

        public DbSet<CommissionRule> CommissionRules { get; set; }

        public DbSet<AgentCommission> AgentCommissions { get; set; }

        public DbSet<AgentCommissionPayment> AgentCommissionPayments { get; set; }
    }
}
