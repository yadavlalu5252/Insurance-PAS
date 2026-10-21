using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Domain.Entities
{
    public class CkycDetails
    {
        [Key]
        public int ckyc_id { get; set; }

        [Required]
        public int customer_id { get; set; }

        [MaxLength(30)]
        public string? ckyc_number { get; set; }

        public DateTime? verification_date { get; set; }

        [Required]
        [MaxLength(20)]
        public string ckyc_status { get; set; } = "Pending";
    }
}
