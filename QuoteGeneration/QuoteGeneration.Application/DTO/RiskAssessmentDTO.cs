using System.ComponentModel.DataAnnotations;

namespace QuoteGeneration.Application.DTO
{
    public class RiskAssessmentDTO
    {
        public int risk_id { get; set; }

        [Required]
        public int quote_id { get; set; }

        public int? age_score { get; set; }

        public int? occupation_score { get; set; }

        public int? medical_score { get; set; }

        public int? lifestyle_score { get; set; }

        public int? total_score { get; set; }

        public string? risk_category { get; set; }
    }
}