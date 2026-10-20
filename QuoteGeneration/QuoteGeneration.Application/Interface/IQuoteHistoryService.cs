using QuoteGeneration.Application.DTO;

namespace QuoteGeneration.Application.Interfaces
{
    public interface IQuoteHistoryService
    {
        Task<List<QuoteHistoryDTO>> GetHistoryByQuoteIdAsync(int quoteId);
        Task CreateHistoryAsync(int quoteId, string action, string? remarks = null, int? performedBy = null);
    }
}