
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Infrastructure.Data;

namespace QuoteGeneration.Infrastructure.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public DiscountService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<DiscountMasterDTO>> GetActiveDiscountsAsync()
        {
            var today = DateTime.UtcNow.Date;

            var discounts = await db.DiscountMasters
                .Where(d => d.status == "Active"
                    && d.valid_from <= today
                    && d.valid_to >= today)
                .ToListAsync();

            return mapper.Map<List<DiscountMasterDTO>>(discounts);
        }
    }
}
