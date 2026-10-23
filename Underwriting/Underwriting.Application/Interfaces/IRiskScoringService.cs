
using Underwriting.Application.DTO;

namespace Underwriting.Application.Interfaces
{
    public interface IRiskScoringService
    {
        Task<riskscoredto?> CalculateRiskScore(int underwritingId);
    }
}
