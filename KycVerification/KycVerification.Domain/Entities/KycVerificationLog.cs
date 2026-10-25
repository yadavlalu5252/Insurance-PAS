using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KycVerification.Domain.Entities
{
    [Table("kyc_verification_log")]
    public class KycVerificationLog
    {
        [Key]
        [Column("log_id")]
        public int log_id { get; set; }

        [Required]
        [Column("kyc_id")]
        public int kyc_id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("action")]
        public string action { get; set; } = string.Empty;

        [Column("performed_by")]
        public int? performed_by { get; set; }

        [Column("action_date")]
        public DateTime action_date { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        [Column("remarks")]
        public string? remarks { get; set; }
    }
}