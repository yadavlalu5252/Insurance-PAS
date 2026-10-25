using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KycVerification.Domain.Entities
{
    [Table("document_type_master")]
    public class DocumentTypeMaster
    {
        [Key]
        [Column("document_type_id")]
        public int DocumentTypeId { get; set; }

        [MaxLength(100)]
        [Column("document_name")]
        public string? DocumentName { get; set; }

        [Column("mandatory_flag")]
        public bool MandatoryFlag { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Active";
    }
}