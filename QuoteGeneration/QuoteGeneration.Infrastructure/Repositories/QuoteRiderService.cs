
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Domain.Entities;
using QuoteGeneration.Infrastructure.Config;
using QuoteGeneration.Infrastructure.Data;

namespace QuoteGeneration.Infrastructure.Services
{
    public class QuoteRiderService : IQuoteRiderService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly InsuranceProductClient productClient;

        public QuoteRiderService(
            AppDbContext db,
            IMapper mapper,
            InsuranceProductClient productClient)
        {
            this.db = db;
            this.mapper = mapper;
            this.productClient = productClient;
        }

        public async Task<QuoteRiderDTO> AddRiderAsync(QuoteRiderDTO dto)
        {
            var quote = await db.Quotes
                .FirstOrDefaultAsync(q => q.quote_id == dto.quote_id);

            if (quote == null)
                throw new Exception("Quote not found.");

            var rider = await productClient.GetRiderByIdAsync(dto.rider_id);

            if (rider == null)
                throw new Exception("Rider not found in Product Management.");

            if (rider.product_id != quote.product_id)
                throw new Exception("Rider does not belong to this quote's product.");

            if (rider.status != "Active")
                throw new Exception("Rider is not active.");

            var quoteRider = mapper.Map<QuoteRider>(dto);

           
            quoteRider.rider_premium = rider.rider_premium;

            db.QuoteRiders.Add(quoteRider);
            await db.SaveChangesAsync();

            return mapper.Map<QuoteRiderDTO>(quoteRider);
        }

        public async Task<List<QuoteRiderDTO>> GetRidersByQuoteIdAsync(int quoteId)
        {
            var riders = await db.QuoteRiders
                .Where(r => r.quote_id == quoteId)
                .ToListAsync();

            return mapper.Map<List<QuoteRiderDTO>>(riders);
        }

        public async Task<bool> RemoveRiderAsync(int id)
        {
            var rider = await db.QuoteRiders
                .FirstOrDefaultAsync(r => r.rider_selection_id == id);

            if (rider == null)
                return false;

            db.QuoteRiders.Remove(rider);
            await db.SaveChangesAsync();

            return true;
        }
    }
}
