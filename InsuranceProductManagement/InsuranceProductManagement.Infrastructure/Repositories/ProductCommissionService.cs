
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Domain.Entities;
using InsuranceProductManagement.Infrastructure.Data;

namespace InsuranceProductManagement.Infrastructure.Repositories
{
    public class ProductCommissionService : IProductCommissionService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public ProductCommissionService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<ProductCommissionDTO>> GetAllAsync()
        {
            var commissions = await db.ProductCommissions.ToListAsync();
            return mapper.Map<List<ProductCommissionDTO>>(commissions);
        }

        public async Task<ProductCommissionDTO?> GetByIdAsync(int id)
        {
            var commission = await db.ProductCommissions
                .FirstOrDefaultAsync(x => x.commission_id == id);

            if (commission == null)
                return null;

            return mapper.Map<ProductCommissionDTO>(commission);
        }

        public async Task<ProductCommissionDTO> CreateAsync(ProductCommissionDTO dto)
        {
            var commission = mapper.Map<ProductCommission>(dto);

            await db.ProductCommissions.AddAsync(commission);
            await db.SaveChangesAsync();

            return mapper.Map<ProductCommissionDTO>(commission);
        }

        public async Task<ProductCommissionDTO?> UpdateAsync(int id, ProductCommissionDTO dto)
        {
            var commission = await db.ProductCommissions
                .FirstOrDefaultAsync(x => x.commission_id == id);

            if (commission == null)
                return null;

            mapper.Map(dto, commission);
            await db.SaveChangesAsync();

            return mapper.Map<ProductCommissionDTO>(commission);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var commission = await db.ProductCommissions
                .FirstOrDefaultAsync(x => x.commission_id == id);

            if (commission == null)
                return false;

            db.ProductCommissions.Remove(commission);
            await db.SaveChangesAsync();

            return true;
        }
    }
}
