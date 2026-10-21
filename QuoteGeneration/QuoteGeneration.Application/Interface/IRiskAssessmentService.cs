using QuoteGeneration.Application.DTO;

namespace QuoteGeneration.Application.Interfaces
{
    public interface IRiskAssessmentService
    {
        Task<RiskAssessmentDTO> AssessRiskAsync(RiskAssessmentDTO dto);
        Task<RiskAssessmentDTO?> GetRiskByQuoteIdAsync(int quoteId);
    }
}