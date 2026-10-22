using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Domain.Entities
{
    public class KycDocument
    {
        [Key]
        public int document_id { get; set; }

        [Required]
        public int kyc_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? document_type { get; set; }

        [Required]
        [MaxLength(100)]
        public string? document_number { get; set; } 

        [Required]
        [MaxLength(500)]
        public string? file_path { get; set; }

        public DateTime upload_date { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string verification_status { get; set; } = "Pending";

    }
}
