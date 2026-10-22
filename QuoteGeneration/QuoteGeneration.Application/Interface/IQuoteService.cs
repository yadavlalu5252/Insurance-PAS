using QuoteGeneration.Application.DTO;

namespace QuoteGeneration.Application.Interfaces
{
    public interface IQuoteService
    {
        Task<QuoteDTO?> CreateQuoteAsync(QuoteDTO dto);
        Task<QuoteDTO?> GetQuoteByIdAsync(int id);
        Task<List<QuoteDTO>> GetAllQuotesAsync();
    }
}