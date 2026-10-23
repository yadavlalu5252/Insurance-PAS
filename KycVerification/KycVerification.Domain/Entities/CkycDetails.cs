using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KycVerification.Domain.Entities
{
    [Table("ckyc_details")]
    public class CkycDetails
    {
        [Key]
        [Column("ckyc_id")]
        public int CkycId { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [MaxLength(30)]
        [Column("ckyc_number")]
        public string? CkycNumber { get; set; }

        [Column("verification_date")]
        public DateTime? VerificationDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("ckyc_status")]
        public string CkycStatus { get; set; } = "Pending";
    }
}