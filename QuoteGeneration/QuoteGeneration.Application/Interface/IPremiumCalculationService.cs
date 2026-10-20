using QuoteGeneration.Application.DTO;

namespace QuoteGeneration.Application.Interfaces
{
    public interface IPremiumCalculationService
    {
        Task<PremiumCalculationDTO?> CalculatePremiumAsync(int quoteId);
        Task<PremiumCalculationDTO?> GetPremiumByQuoteIdAsync(int quoteId);
    }
}