using System.ComponentModel.DataAnnotations;

namespace InsuranceProductManagement.Application.DTO
{
    public class PremiumFrequencyDTO
    {
        public int frequency_id { get; set; }

        [Required]
        public string? frequency_name { get; set; }

        public int? installment_count { get; set; }

        public string? status { get; set; } = "Active";
    }
}