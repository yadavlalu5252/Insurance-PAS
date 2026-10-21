using InvestigationService.Application.DTOs;

namespace InvestigationService.Application.Interface
{
    public interface IInvestigationService
    {
        Task AddInvestigation(long claimId, CreateInvestigationDto dto);
        Task SubmitInvestigation(long investigationId,SubmitInvestigationDto dto);
    }
}