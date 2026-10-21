using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Domain.Entities
{
    public class KycVerificationLog
    {
        [Key]
        public int log_id { get; set; }

        [Required]
        public int kyc_id { get; set; }

        [Required]
        [MaxLength(100)]
        public string action { get; set; } = string.Empty;

        public int? performed_by { get; set; }

        public DateTime action_date { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? remarks { get; set; }
    }
}
