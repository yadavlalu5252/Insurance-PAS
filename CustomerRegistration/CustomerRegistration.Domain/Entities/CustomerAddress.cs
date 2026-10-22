
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerRegistration.Domain.Entities;

[Table("customer_address")]
public class CustomerAddress
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long address_id { get; set; }

    public long customer_id { get; set; }

    [Required]
    [StringLength(20)]
    public string address_type { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string address_line1 { get; set; } = string.Empty;

    [StringLength(255)]
    public string? address_line2 { get; set; }

    [Required]
    [StringLength(100)]
    public string city { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string district { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string state { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string country { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string pincode { get; set; } = string.Empty;

    [ForeignKey(nameof(customer_id))]
    public Customer? customer { get; set; }
}
