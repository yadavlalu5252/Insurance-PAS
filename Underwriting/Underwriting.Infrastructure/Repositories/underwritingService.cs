
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;
using Underwriting.Infrastructure.Data;

namespace Underwriting.Infrastructure.Repositories
{
    public class underwritingService : Iunderwriting
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public underwritingService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task addunerwriting(createunderwritingdto dto)
        {
            var data = mapper.Map<underwriting_case>(dto);

            data.underwriting_date = DateTime.Now;
            data.status = "Pending";

            await db.underwriting_case.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task<List<underwriting_case>> fetchunderwriting()
        {
            var data = await db.underwriting_case.ToListAsync();

            return data;
        }

        public async Task<underwriting_case?> fetchunderwritingbyid(int id)
        {
            var data = await db.underwriting_case.FindAsync(id);

            return data;
        }

        public async Task updateunderwritingbyid( int id,createunderwritingdto dto)
        {
            var data = await db.underwriting_case.FindAsync(id);

            if (data != null)
            {
                mapper.Map(dto, data);

                await db.SaveChangesAsync();
            }
        }

        public async Task approveunderwriting(int id)
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

        public async Task rejectunderwriting(int id, string reason)
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

        public async Task loadingunderwriting(int id)
        {
            var data = await db.underwriting_case.FindAsync(id);

            if (data != null)
            {
                var risk = await db.risk_score .FirstOrDefaultAsync(x => x.underwriting_id == id);

                if (risk != null)
                {
                    decimal loading = 0;

                    if (risk.risk_category == "Low Risk")
                    {
                        loading = 0;
                    }
                    else if (risk.risk_category == "Medium Risk")
                    {
                        loading = 10;
                    }
                    else if (risk.risk_category == "High Risk")
                    {
                        loading = 25;
                    }
                    else if (risk.risk_category == "Very High Risk")
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

        public async Task<underwriting_decision?> fetchdecision(int id)
        {
            var data = await db.underwriting_decision
                .FirstOrDefaultAsync(x => x.underwriting_id == id);

            return data;
        }
        public async Task<underwriting_case?> fetchunderwritingbyquoteid(int quoteId)
        {
            return await db.underwriting_case
                .FirstOrDefaultAsync(x => x.quote_id == quoteId);
        }
    }
}
