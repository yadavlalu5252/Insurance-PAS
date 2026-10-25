using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KycVerification.Domain.Entities
{
    [Table("kyc_document")]
    public class KycDocument
    {
        [Key]
        [Column("document_id")]
        public int DocumentId { get; set; }

        [Required]
        [Column("kyc_id")]
        public int KycId { get; set; }

        [MaxLength(50)]
        [Column("document_type")]
        public string? DocumentType { get; set; }

        [MaxLength(50)]
        [Column("document_number")]
        public string? DocumentNumber { get; set; }

        [MaxLength(500)]
        [Column("file_path")]
        public string? FilePath { get; set; }

        [Column("upload_date")]
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        [Column("verification_status")]
        public string VerificationStatus { get; set; } = "Pending";
    }
}