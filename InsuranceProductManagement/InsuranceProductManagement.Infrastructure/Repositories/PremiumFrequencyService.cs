
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Domain.Entities;
using InsuranceProductManagement.Infrastructure.Data;

namespace InsuranceProductManagement.Infrastructure.Repositories
{
    public class PremiumFrequencyService : IPremiumFrequencyService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public PremiumFrequencyService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<PremiumFrequencyDTO>> GetAllAsync()
        {
            var frequencies = await db.PremiumFrequencies.ToListAsync();
            return mapper.Map<List<PremiumFrequencyDTO>>(frequencies);
        }

        public async Task<PremiumFrequencyDTO?> GetByIdAsync(int id)
        {
            var frequency = await db.PremiumFrequencies .FirstOrDefaultAsync(x => x.frequency_id == id);

            if (frequency == null)
                return null;

            return mapper.Map<PremiumFrequencyDTO>(frequency);
        }

        public async Task<PremiumFrequencyDTO> CreateAsync(PremiumFrequencyDTO dto)
        {
            var frequency = mapper.Map<PremiumFrequency>(dto);

            await db.PremiumFrequencies.AddAsync(frequency);
            await db.SaveChangesAsync();

            return mapper.Map<PremiumFrequencyDTO>(frequency);
        }

        public async Task<PremiumFrequencyDTO?> UpdateAsync(int id, PremiumFrequencyDTO dto)
        {
            var frequency = await db.PremiumFrequencies.FirstOrDefaultAsync(x => x.frequency_id == id);

            if (frequency == null)
                return null;

            mapper.Map(dto, frequency);
            await db.SaveChangesAsync();

            return mapper.Map<PremiumFrequencyDTO>(frequency);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var frequency = await db.PremiumFrequencies.FirstOrDefaultAsync(x => x.frequency_id == id);

            if (frequency == null)
                return false;

            frequency.status = "Inactive";
            await db.SaveChangesAsync();

            return true;
        }
    }
}
