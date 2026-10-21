
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Domain.Entities;
using InsuranceProductManagement.Infrastructure.Data;

namespace InsuranceProductManagement.Infrastructure.Repositories
{
    public class BenefitMasterService : IBenefitMasterService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public BenefitMasterService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<BenefitMasterDTO>> GetAllAsync()
        {
            var benefits = await db.BenefitMasters.ToListAsync();
            return mapper.Map<List<BenefitMasterDTO>>(benefits);
        }

        public async Task<BenefitMasterDTO?> GetByIdAsync(int id)
        {
            var benefit = await db.BenefitMasters
                .FirstOrDefaultAsync(x => x.benefit_id == id);

            if (benefit == null)
                return null;

            return mapper.Map<BenefitMasterDTO>(benefit);
        }

        public async Task<BenefitMasterDTO> CreateAsync(BenefitMasterDTO dto)
        {
            var benefit = mapper.Map<BenefitMaster>(dto);

            await db.BenefitMasters.AddAsync(benefit);
            await db.SaveChangesAsync();

            return mapper.Map<BenefitMasterDTO>(benefit);
        }

        public async Task<BenefitMasterDTO?> UpdateAsync(int id, BenefitMasterDTO dto)
        {
            var benefit = await db.BenefitMasters
                .FirstOrDefaultAsync(x => x.benefit_id == id);

            if (benefit == null)
                return null;

            mapper.Map(dto, benefit);
            await db.SaveChangesAsync();

            return mapper.Map<BenefitMasterDTO>(benefit);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var benefit = await db.BenefitMasters
                .FirstOrDefaultAsync(x => x.benefit_id == id);

            if (benefit == null)
                return false;

            db.BenefitMasters.Remove(benefit);
            await db.SaveChangesAsync();

            return true;
        }
    }
}
