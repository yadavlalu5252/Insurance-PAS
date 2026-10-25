
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;
using Underwriting.Infrastructure.Data;

namespace Underwriting.Infrastructure.Repositories
{
    public class savefinancialservice : Isavefinancial
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public savefinancialservice(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task addfinancial(int id, savefinancialdto dto)
        {
            var mdata = await db.underwriting_case.FindAsync(id);

            if (mdata != null)
            {
                var data = mapper.Map<financial_underwriting>(dto);

                data.underwriting_id = id;

                await db.financial_underwriting.AddAsync(data);
                await db.SaveChangesAsync();
            }
        }

        public async Task<financial_underwriting?> fetchfinancial(int id)
        {
            var data = await db.financial_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);

            return data;
        }

        public async Task updatefinancial(int id, savefinancialdto dto)
        {
            var data = await db.financial_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);

            if (data != null)
            {
                mapper.Map(dto, data);

                await db.SaveChangesAsync();
            }
        }
    }
}
