
using Microsoft.EntityFrameworkCore;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;
using Underwriting.Infrastructure.Data;

namespace Underwriting.Infrastructure.Repositories
{
    public class underwritingService : Iunderwriting
    {
        AppDbContext db;

        public underwritingService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task addunerwriting(createunderwritingdto dto)
        {
            var data = new underwriting_case()
            {
                quote_id = dto.quote_id,
                customer_id = dto.customer_id,
                product_id = dto.product_id,
                underwriting_date = DateTime.Now,
                status = "Pending"
            };

            await db.underwriting_case.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task<List<underwriting_case>> fetchunderwriting()
        {
            var data = await db.underwriting_case.ToListAsync();
            return data;
        }

        public async Task<underwriting_case?> fetchunderwritingbyid(long id)
        {
            var data = await db.underwriting_case.FindAsync(id);
            return data;
        }

        public async Task updateunderwritingbyid(long id, createunderwritingdto dto)
        {
            var data = await db.underwriting_case.FindAsync(id);

            if (data != null)
            {
                data.quote_id = dto.quote_id;
                data.customer_id = dto.customer_id;
                data.product_id = dto.product_id;
                await db.SaveChangesAsync();
            }
        }
        public async Task approveunderwriting(long id)
        {
            var data = await db.underwriting_case.FindAsync(id);
            if (data != null)
            {
                data.status = "Approved";
                var decision = new underwriting_decision()
                {
                    underwriting_id = id,
                    decision = "Approved",
                    loading_percentage = 0,
                    decision_date = DateTime.Now
                };

                var audit = new underwriting_audit_log()
                {
                    underwriting_id = id,
                    action = "Approved",
                    remarks = "Underwriting case approved",
                    action_date = DateTime.Now
                };

                await db.underwriting_decision.AddAsync(decision);
                await db.underwriting_audit_log.AddAsync(audit);
                await db.SaveChangesAsync();
            }
        }
        public async Task rejectunderwriting(long id, string reason)
        {
            var data = await db.underwriting_case.FindAsync(id);

            if (data != null)
            {
                data.status = "Rejected";

                var decision = new underwriting_decision()
                {
                    underwriting_id = id,
                    decision = "Rejected",
                    rejection_reason = reason,
                    loading_percentage = 0,
                    decision_date = DateTime.Now
                };

                var audit = new underwriting_audit_log()
                {
                    underwriting_id = id,
                    action = "Rejected",
                    remarks = reason,
                    action_date = DateTime.Now
                };

                await db.underwriting_decision.AddAsync(decision);
                await db.underwriting_audit_log.AddAsync(audit);

                await db.SaveChangesAsync();
            }
        }
        public async Task loadingunderwriting(long id)
        {
            var data = await db.underwriting_case.FindAsync(id);

            if (data != null)
            {
                var risk = await db.risk_score.FirstOrDefaultAsync(x => x.underwriting_id == id);
                if (risk != null)
                {
                    decimal loading = 0;

                    if (risk.risk_category == "Low")
                    {
                        loading = 0;
                    }
                    else if (risk.risk_category == "Medium")
                    {
                        loading = 10;
                    }
                    else if (risk.risk_category == "High")
                    {
                        loading = 25;
                    }
                    else if (risk.risk_category == "Very High")
                    {
                        loading = 50;
                    }

                    var decision = await db.underwriting_decision.FirstOrDefaultAsync(x => x.underwriting_id == id);
                    if (decision != null && decision.decision != "Rejected")
                    {
                        decision.loading_percentage = loading;

                        if (loading > 0)
                        {
                            decision.decision = "Approved with Loading";
                        }
                        else
                        {
                            decision.decision = "Approved";
                        }

                        var audit = new underwriting_audit_log()
                        {
                            underwriting_id = id,
                            action = "Premium Loading Applied",
                            remarks = "Loading percentage: " + loading,
                            action_date = DateTime.Now
                        };

                        await db.underwriting_audit_log.AddAsync(audit);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
        public async Task<underwriting_decision?> fetchdecision(long id)
        {
            var data = await db.underwriting_decision.FirstOrDefaultAsync(x => x.underwriting_id == id);
            return data;
        }
    }
}
