using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Infrastructure.Data;
using QuoteGeneration.Domain.Entities;

namespace QuoteGeneration.Infrastructure.Services
{
    public class QuoteHistoryService : IQuoteHistoryService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public QuoteHistoryService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task CreateHistoryAsync(int quoteId, string action, string? remarks = null, int? performedBy = null)
        {
            var history = new QuoteHistory
            {
                quote_id = quoteId,
                action = action,
                remarks = remarks,
                performed_by = performedBy,
                action_date = DateTime.UtcNow
            };

            db.QuoteHistories.Add(history);
            await db.SaveChangesAsync();
        }

        public async Task<List<QuoteHistoryDTO>> GetHistoryByQuoteIdAsync(int quoteId)
        {
            var history = await db.QuoteHistories
                .Where(h => h.quote_id == quoteId)
                .OrderByDescending(h => h.action_date)
                .ToListAsync();

            return mapper.Map<List<QuoteHistoryDTO>>(history);
        }
    }
}