
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Domain.Entities;
using InsuranceProductManagement.Infrastructure.Data;

namespace InsuranceProductManagement.Infrastructure.Repositories
{
    public class InsuranceProductService : IInsuranceProductService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public InsuranceProductService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<InsuranceProductDTO>> GetAllAsync()
        {
            var products = await db.InsuranceProducts.ToListAsync();

            return mapper.Map<List<InsuranceProductDTO>>(products);
        }

        public async Task<InsuranceProductDTO?> GetByIdAsync(int id)
        {
            var product = await db.InsuranceProducts
                .FirstOrDefaultAsync(x => x.product_id == id);

            if (product == null)
            {
                return null;
            }

            return mapper.Map<InsuranceProductDTO>(product);
        }

        public async Task<InsuranceProductDTO> CreateAsync(InsuranceProductDTO dto)
        {
            var product = mapper.Map<InsuranceProduct>(dto);

            await db.InsuranceProducts.AddAsync(product);
            await db.SaveChangesAsync();

            return mapper.Map<InsuranceProductDTO>(product);
        }

        public async Task<InsuranceProductDTO?> UpdateAsync(int id, InsuranceProductDTO dto)
        {
            var product = await db.InsuranceProducts
                .FirstOrDefaultAsync(x => x.product_id == id);

            if (product == null)
            {
                return null;
            }

            mapper.Map(dto, product);

            await db.SaveChangesAsync();

            return mapper.Map<InsuranceProductDTO>(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await db.InsuranceProducts
                .FirstOrDefaultAsync(x => x.product_id == id);

            if (product == null)
            {
                return false;
            }

            product.status = "Inactive";

            await db.SaveChangesAsync();

            return true;
        }


        public async Task<List<RiderMasterDTO>> GetRidersByProductIdAsync(long productId)
        {
            // Assuming he has a mapping table or a direct FK. Adjust based on his DB schema.
            var riders = await db.RiderMasters
                .Where(r => r.product_id == productId && r.status == "Active")
                .ToListAsync();

            // If he uses AutoMapper:
            return mapper.Map<List<RiderMasterDTO>>(riders);
        }

        public async Task<List<CoverageMasterDTO>> GetCoveragesByProductIdAsync(long productId)
        {
            var coverages = await db.CoverageMasters
                .Where(c => c.product_id == productId)
                .ToListAsync();

            // If he uses AutoMapper:
            return mapper.Map<List<CoverageMasterDTO>>(coverages);
        }
    }
}
