
using System.ComponentModel.DataAnnotations;

namespace Underwriting.Application.DTO
{
    public class riskscoredto
    {
        [Required]
        public int underwriting_id { get; set; }

        public int age_score { get; set; }

        public int medical_score { get; set; }

        public int occupation_score { get; set; }

        public int lifestyle_score { get; set; }

        public int financial_score { get; set; }

        public int total_score { get; set; }

        public string ?risk_category { get; set; }
    }
}
