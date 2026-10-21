
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerRegistration.Domain.Entities;

[Table("customer_document")]
public class CustomerDocument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long document_id { get; set; }

    public long customer_id { get; set; }

    [Required]
    [StringLength(100)]
    public string document_type { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string file_name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string file_path { get; set; } = string.Empty;

    public DateTime uploaded_date { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(20)]
    public string verification_status { get; set; } = "Pending";

    [ForeignKey(nameof(customer_id))]
    public Customer? customer { get; set; }
}
