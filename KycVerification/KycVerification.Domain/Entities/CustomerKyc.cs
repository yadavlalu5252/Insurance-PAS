using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KycVerification.Domain.Entities
{
    [Table("customer_kyc")]
    public class CustomerKyc
    {
        [Key]
        [Column("kyc_id")]
        public int kyc_id { get; set; }

        [Column("customer_id")]
        public int customer_id { get; set; }

        [MaxLength(50)]
        [Column("kyc_number")]
        public string? kyc_number { get; set; }

        [Column("kyc_date")]
        public DateTime kyc_date { get; set; }

        [MaxLength(20)]
        [Column("verification_status")]
        public string verification_status { get; set; } = "Pending";

        [MaxLength(500)]
        [Column("remarks")]
        public string? remarks { get; set; }

        [MaxLength(100)]
        [Column("verified_by")]
        public string? verified_by { get; set; }

        [Column("verified_date")]
        public DateTime? verified_date { get; set; }
    }
}