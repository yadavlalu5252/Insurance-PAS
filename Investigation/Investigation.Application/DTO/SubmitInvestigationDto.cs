using Microsoft.AspNetCore.Http;

namespace InvestigationService.Application.DTOs
{
    public class SubmitInvestigationDto
    {
        public IFormFile ReportFile { get; set; } = null!;
    }
}