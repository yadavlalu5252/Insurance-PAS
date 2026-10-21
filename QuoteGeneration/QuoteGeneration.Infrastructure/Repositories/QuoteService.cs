
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Domain.Entities;
using QuoteGeneration.Infrastructure.Config;
using QuoteGeneration.Infrastructure.Data;

namespace QuoteGeneration.Infrastructure.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly InsuranceProductClient productClient;
        private readonly IQuoteHistoryService quoteHistoryService;

        public QuoteService(AppDbContext db, IMapper mapper, InsuranceProductClient productClient, IQuoteHistoryService quoteHistoryService)
        {
            this.db = db;
            this.mapper = mapper;
            this.productClient = productClient;
            this.quoteHistoryService = quoteHistoryService;
        }


        public async Task<QuoteDTO?> CreateQuoteAsync(QuoteDTO dto)
        {
            var product = await productClient.GetProductByIdAsync(dto.product_id);

            if (product == null)
            {
                return null;
            }

            if (!string.Equals(product.status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (product.minimum_sum_insured.HasValue &&dto.sum_insured < product.minimum_sum_insured.Value)
            {
                return null;
            }

            if (product.maximum_sum_insured.HasValue &&dto.sum_insured > product.maximum_sum_insured.Value)
            {
                return null;
            }

            var quote = mapper.Map<Quote>(dto);

            quote.quote_date = DateTime.UtcNow;
            quote.expiry_date = quote.quote_date.AddDays(30);
            quote.quote_status = "Pending";

            db.Quotes.Add(quote);
            await db.SaveChangesAsync();

            quote.quote_number = "QT-" + quote.quote_id.ToString("D6");

            await db.SaveChangesAsync();

            await quoteHistoryService.CreateHistoryAsync(quote.quote_id,"Quote Created", "Quote created successfully.");

            return mapper.Map<QuoteDTO>(quote);
        }



        public async Task<QuoteDTO?> GetQuoteByIdAsync(int id)
        {
            var quote = await db.Quotes.FirstOrDefaultAsync(q => q.quote_id == id);

            if (quote == null)
            {
                return null;
            }

            return mapper.Map<QuoteDTO>(quote);
        }

      
        public async Task<List<QuoteDTO>> GetAllQuotesAsync()
        {
            var quotes = await db.Quotes.OrderByDescending(q => q.quote_id) .ToListAsync();

            return mapper.Map<List<QuoteDTO>>(quotes);
        }
    }
}
