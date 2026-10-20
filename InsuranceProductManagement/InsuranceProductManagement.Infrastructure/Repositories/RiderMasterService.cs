
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Domain.Entities;
using InsuranceProductManagement.Infrastructure.Data;

namespace InsuranceProductManagement.Infrastructure.Repositories
{
    public class RiderMasterService : IRiderMasterService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public RiderMasterService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<RiderMasterDTO>> GetAllAsync()
        {
            var riders = await db.RiderMasters.ToListAsync();
            return mapper.Map<List<RiderMasterDTO>>(riders);
        }

        public async Task<RiderMasterDTO?> GetByIdAsync(int id)
        {
            var rider = await db.RiderMasters
                .FirstOrDefaultAsync(x => x.rider_id == id);

            if (rider == null)
                return null;

            return mapper.Map<RiderMasterDTO>(rider);
        }

        public async Task<RiderMasterDTO> CreateAsync(RiderMasterDTO dto)
        {
            var rider = mapper.Map<RiderMaster>(dto);

            await db.RiderMasters.AddAsync(rider);
            await db.SaveChangesAsync();

            return mapper.Map<RiderMasterDTO>(rider);
        }

        public async Task<RiderMasterDTO?> UpdateAsync(int id, RiderMasterDTO dto)
        {
            var rider = await db.RiderMasters
                .FirstOrDefaultAsync(x => x.rider_id == id);

            if (rider == null)
                return null;

            mapper.Map(dto, rider);
            await db.SaveChangesAsync();

            return mapper.Map<RiderMasterDTO>(rider);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rider = await db.RiderMasters
                .FirstOrDefaultAsync(x => x.rider_id == id);

            if (rider == null)
                return false;

            rider.status = "Inactive";
            await db.SaveChangesAsync();

            return true;
        }
    }
}
