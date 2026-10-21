namespace InvestigationService.Application.DTOs
{
    public class CreateInvestigationDto
    {
        public long ClaimId { get; set; }

        public long InvestigatorId { get; set; }

        public string InvestigationReason { get; set; } = string.Empty;
    }
}