
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;
using Underwriting.Infrastructure.Data;

namespace Underwriting.Infrastructure.Repositories
{
    public class savemedicalServices : Isavemedical
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public savemedicalServices(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task addmedical(int id, savemedicaldto dto)
        {
            var mdata = await db.underwriting_case.FindAsync(id);

            if (mdata != null)
            {
                var data = mapper.Map<medical_underwriting>(dto);

                data.underwriting_id = id;

                await db.medical_underwriting.AddAsync(data);
                await db.SaveChangesAsync();
            }
        }

        public async Task<medical_underwriting?> fetchmedical(int id)
        {
            var data = await db.medical_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);

            return data;
        }

        public async Task updatemedical(int id, savemedicaldto dto)
        {
            var data = await db.medical_underwriting.FirstOrDefaultAsync(x => x.underwriting_id == id);

            if (data != null)
            {
                mapper.Map(dto, data);

                await db.SaveChangesAsync();
            }
        }
    }
}
