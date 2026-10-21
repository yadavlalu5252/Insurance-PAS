using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Domain.Entities
{
    public class CustomerKyc
    {
        [Key]
        public int kyc_id { get; set; }

        [Required]
        public int customer_id { get; set; }

        [Required]
        [MaxLength(30)]
        public string? kyc_number { get; set; } 

        [Required]
        public DateTime kyc_date { get; set; }

        [Required]
        [MaxLength(20)]
        public string verification_status { get; set; } = "Pending";

        [MaxLength(500)]
        public string? remarks { get; set; }

        public int? verified_by { get; set; }

        public DateTime? verified_date { get; set; }
    }
}
