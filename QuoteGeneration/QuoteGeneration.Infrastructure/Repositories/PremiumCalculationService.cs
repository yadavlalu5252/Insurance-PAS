using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;
using QuoteGeneration.Infrastructure.Config;
using QuoteGeneration.Infrastructure.Data;
using InsuranceProductManagement.Domain.Entities;

namespace QuoteGeneration.Infrastructure.Services
{
    public class PremiumCalculationService : IPremiumCalculationService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly InsuranceProductClient productClient;
        private readonly IQuoteHistoryService quoteHistoryService;
        private readonly UnderwritingClient underwritingClient;

        public PremiumCalculationService(AppDbContext db,IMapper mapper,InsuranceProductClient productClient,IQuoteHistoryService quoteHistoryService,UnderwritingClient underwritingClient)
        {
            this.db = db;
            this.mapper = mapper;
            this.productClient = productClient;
            this.quoteHistoryService = quoteHistoryService;
            this.underwritingClient = underwritingClient;
        }

        public async Task<PremiumCalculationDTO?> CalculatePremiumAsync(int quoteId)
        {
            var quote = await db.Quotes.FirstOrDefaultAsync(q => q.quote_id == quoteId);

            if (quote == null)
            {
                return null;
            }

            var product = await productClient.GetProductByIdAsync(quote.product_id);

            if (product == null || !string.Equals(product.status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if ((product.minimum_sum_insured.HasValue && quote.sum_insured < product.minimum_sum_insured.Value) || (product.maximum_sum_insured.HasValue && quote.sum_insured > product.maximum_sum_insured.Value))
            {
                return null;
            }

            decimal productRate;

            if (string.Equals(product.product_type, "Health", StringComparison.OrdinalIgnoreCase))
            {
                productRate = 1.1m;
            }
            else if (string.Equals(product.product_type, "Life", StringComparison.OrdinalIgnoreCase))
            {
                productRate = 1.2m;
            }
            else if (string.Equals(product.product_type, "Vehicle", StringComparison.OrdinalIgnoreCase))
            {
                productRate = 1.3m;
            }
            else
            {
                return null;
            }


            var underwriting = await underwritingClient.GetRiskByQuoteIdAsync(quoteId);

            if (underwriting == null)
            {
                return null;
            }

            decimal riskPercentage;

            if (underwriting.risk_category == "Low Risk")
                riskPercentage = 0m;
            else if (underwriting.risk_category == "Medium Risk")
                riskPercentage = 10m;
            else if (underwriting.risk_category == "High Risk")
                riskPercentage = 25m;
            else if (underwriting.risk_category == "Very High Risk")
                riskPercentage = 50m;
            else
                return null;

            decimal gstPercentage = 18m;



            var selectedRiders = await db.QuoteRiders.Where(qr => qr.quote_id == quoteId).ToListAsync();

            var riderIds = selectedRiders.Select(qr => qr.rider_id) .ToList();

            var activeRiders = await db.Set<RiderMaster>().Where(r => riderIds.Contains(r.rider_id) && r.product_id == quote.product_id && r.status == "Active").ToListAsync();

            decimal riderPremium = activeRiders.Sum(r => r.rider_premium ?? 0m);

            foreach (var selected in selectedRiders)
            {
                var rider = activeRiders.FirstOrDefault(r => r.rider_id == selected.rider_id);

                if (rider != null)
                {
                    selected.rider_premium = rider.rider_premium ?? 0m;
                }
            }

            var today = DateTime.UtcNow.Date;

            var discount = await db.DiscountMasters
                .Where(d => d.status == "Active" &&
                            (!d.valid_from.HasValue || d.valid_from.Value.Date <= today) &&
                            (!d.valid_to.HasValue || d.valid_to.Value.Date >= today) &&
                            d.discount_percentage.HasValue)
                .OrderByDescending(d => d.discount_percentage)
                .FirstOrDefaultAsync();

            decimal discountAmount = 0m;

            if (discount != null)
            {
                discountAmount = (quote.sum_insured * productRate / 1000m) * discount.discount_percentage!.Value / 100m;
            }

            decimal basePremium = (quote.sum_insured * productRate) / 1000m;
            decimal riskLoading = basePremium * riskPercentage / 100m;

            decimal premiumBeforeGST = basePremium - discountAmount + riskLoading + riderPremium;

            decimal gstAmount = premiumBeforeGST * gstPercentage / 100m;
            decimal finalPremium = premiumBeforeGST + gstAmount;

            var calculation = await db.PremiumCalculations .FirstOrDefaultAsync(p => p.quote_id == quoteId);

            if (calculation == null)
            {
                calculation = new QuoteGeneration.Domain.Entities.PremiumCalculation
                {
                    quote_id = quoteId
                };

                db.PremiumCalculations.Add(calculation);
            }

            calculation.base_premium = basePremium;
            calculation.risk_loading = riskLoading;
            calculation.rider_premium = riderPremium;
            calculation.discount_amount = discountAmount;
            calculation.gst_amount = gstAmount;
            calculation.final_premium = finalPremium;

            await db.SaveChangesAsync();
            await quoteHistoryService.CreateHistoryAsync(quoteId,"Premium Calculated","Premium calculated successfully.");

            return mapper.Map<PremiumCalculationDTO>(calculation);
        }

        public async Task<PremiumCalculationDTO?> GetPremiumByQuoteIdAsync(int quoteId)
        {
            var calculation = await db.PremiumCalculations.FirstOrDefaultAsync(p => p.quote_id == quoteId);

            if (calculation == null)
            {
                return null;
            }

            return mapper.Map<PremiumCalculationDTO>(calculation);
        }
    }
}