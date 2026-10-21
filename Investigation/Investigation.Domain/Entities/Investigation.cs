using investigation.Domain.Entities.Enums;

namespace InvestigationService.Domain.Entities
{
    public class Investigation
    {
        public long InvestigationId { get; set; }

        public long ClaimId { get; set; }

        public long InvestigatorId { get; set; }

        public string InvestigationReason { get; set; } = string.Empty;

        public string? ReportPath { get; set; }

        public InvestigationStatus InvestigationStatus { get; set; }
    }
}