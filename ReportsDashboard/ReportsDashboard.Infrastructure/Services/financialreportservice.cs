
using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;
using ReportsDashboard.Infrastructure.Config;

namespace ReportsDashboard.Infrastructure.Services
{
    public class financialreportservice : Ifinancialreport
    {
        private readonly PremiumClient premium;
        private readonly ClaimsClient claims;

        public financialreportservice(
            PremiumClient premium,
            ClaimsClient claims)
        {
            this.premium = premium;
            this.claims = claims;
        }

        public async Task<FinancialReportDTO?> getfinancial()
        {
            var premiums = await premium.getpremiums();
            var claimsdata = await claims.getclaims();

            var report = new FinancialReportDTO();

            if (premiums == null || claimsdata == null)
            {
                return null;
            }

            report.premium_income = premiums
                .Where(p => p.status != null &&
                    p.status.Equals("Paid",
                        StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.amount);

            report.claim_payments = claimsdata
                .Sum(c => c.settlement_amount ?? 0);

            report.outstanding_premium = premiums
                .Where(p => p.status != null &&
                    (p.status.Equals("Pending",
                        StringComparison.OrdinalIgnoreCase) ||
                     p.status.Equals("Overdue",
                        StringComparison.OrdinalIgnoreCase)))
                .Sum(p => p.amount);

            report.refunds = premiums
                .Sum(p => p.refund_amount ?? 0);

            report.monthly_profit = premiums
                .Where(p => p.payment_date.HasValue &&
                    p.payment_date.Value.Month == DateTime.Now.Month &&
                    p.payment_date.Value.Year == DateTime.Now.Year &&
                    p.status != null &&
                    p.status.Equals("Paid",
                        StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.amount)
                -
                claimsdata
                .Where(c => c.settlement_date.HasValue &&
                    c.settlement_date.Value.Month == DateTime.Now.Month &&
                    c.settlement_date.Value.Year == DateTime.Now.Year)
                .Sum(c => c.settlement_amount ?? 0);

            // Branch revenue cannot be calculated from
            // PremiumDTO and ClaimsDTO because they have no branch field.
            report.branch_revenue = 0;

            return report;
        }
    }
}
