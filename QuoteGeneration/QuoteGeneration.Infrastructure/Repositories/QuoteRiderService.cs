
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Infrastructure.Data;
using InsuranceProductManagement.Domain.Entities;
using QuoteGeneration.Domain.Entities;

namespace QuoteGeneration.Infrastructure.Services
{
    public class QuoteRiderService : IQuoteRiderService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public QuoteRiderService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<QuoteRiderDTO> AddRiderAsync(QuoteRiderDTO dto)
        {
            var quote = await db.Quotes.FirstOrDefaultAsync(q => q.quote_id == dto.quote_id);

            if (quote == null)
            {
                throw new Exception("Quote not found.");
            }

            var rider = await db.Set<RiderMaster>().FirstOrDefaultAsync(r => r.rider_id == dto.rider_id);

            if (rider == null)
            {
                throw new Exception("Rider not found.");
            }

            var quoteRider = mapper.Map<QuoteRider>(dto);

            db.QuoteRiders.Add(quoteRider);
            await db.SaveChangesAsync();

            return mapper.Map<QuoteRiderDTO>(quoteRider);
        }

        public async Task<List<QuoteRiderDTO>> GetRidersByQuoteIdAsync(int quoteId)
        {
            var riders = await db.QuoteRiders.Where(r => r.quote_id == quoteId).ToListAsync();

            return mapper.Map<List<QuoteRiderDTO>>(riders);
        }

        public async Task<bool> RemoveRiderAsync(int id)
        {
            var rider = await db.QuoteRiders.FirstOrDefaultAsync(r => r.rider_selection_id == id);

            if (rider == null)
            {
                return false;
            }

            db.QuoteRiders.Remove(rider);
            await db.SaveChangesAsync();

            return true;
        }
    }
}
