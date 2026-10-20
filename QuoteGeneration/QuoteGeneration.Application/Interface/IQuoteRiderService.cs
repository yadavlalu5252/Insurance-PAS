using QuoteGeneration.Application.DTO;

namespace QuoteGeneration.Application.Interfaces
{
    public interface IQuoteRiderService
    {
        Task<QuoteRiderDTO> AddRiderAsync(QuoteRiderDTO dto);
        Task<List<QuoteRiderDTO>> GetRidersByQuoteIdAsync(int quoteId);
        Task<bool> RemoveRiderAsync(int id);
    }
}