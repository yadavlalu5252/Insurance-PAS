
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Domain.Entities;
using InsuranceProductManagement.Infrastructure.Data;

namespace InsuranceProductManagement.Infrastructure.Repositories
{
    public class CoverageMasterService : ICoverageMasterService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public CoverageMasterService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<CoverageMasterDTO>> GetAllAsync()
        {
            var coverages = await db.CoverageMasters.ToListAsync();
            return mapper.Map<List<CoverageMasterDTO>>(coverages);
        }

        public async Task<CoverageMasterDTO?> GetByIdAsync(int id)
        {
            var coverage = await db.CoverageMasters
                .FirstOrDefaultAsync(x => x.coverage_id == id);

            if (coverage == null)
                return null;

            return mapper.Map<CoverageMasterDTO>(coverage);
        }

        public async Task<CoverageMasterDTO> CreateAsync(CoverageMasterDTO dto)
        {
            var coverage = mapper.Map<CoverageMaster>(dto);

            await db.CoverageMasters.AddAsync(coverage);
            await db.SaveChangesAsync();

            return mapper.Map<CoverageMasterDTO>(coverage);
        }

        public async Task<CoverageMasterDTO?> UpdateAsync(int id, CoverageMasterDTO dto)
        {
            var coverage = await db.CoverageMasters
                .FirstOrDefaultAsync(x => x.coverage_id == id);

            if (coverage == null)
                return null;

            mapper.Map(dto, coverage);
            await db.SaveChangesAsync();

            return mapper.Map<CoverageMasterDTO>(coverage);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var coverage = await db.CoverageMasters
                .FirstOrDefaultAsync(x => x.coverage_id == id);

            if (coverage == null)
                return false;

            db.CoverageMasters.Remove(coverage);
            await db.SaveChangesAsync();

            return true;
        }
    }
}
