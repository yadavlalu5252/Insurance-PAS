namespace InvestigationService.Application.DTOs
{
    public class InvestigationDetailsDto
    {
        public long InvestigationId { get; set; }

        public long ClaimId { get; set; }

        public long InvestigatorId { get; set; }

        public string InvestigationReason { get; set; } = string.Empty;

        public string? ReportPath { get; set; }

        public string InvestigationStatus { get; set; } = string.Empty;
    }
}