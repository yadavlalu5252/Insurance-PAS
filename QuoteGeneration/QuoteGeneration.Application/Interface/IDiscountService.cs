using QuoteGeneration.Application.DTO;

namespace QuoteGeneration.Application.Interfaces
{
    public interface IDiscountService
    {
        Task<List<DiscountMasterDTO>> GetActiveDiscountsAsync();
    }
}