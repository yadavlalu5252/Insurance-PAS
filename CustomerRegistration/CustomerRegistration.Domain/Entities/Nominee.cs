
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerRegistration.Domain.Entities;

[Table("nominee")]
public class Nominee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long nominee_id { get; set; }

    public long customer_id { get; set; }

    [Required]
    [StringLength(150)]
    public string nominee_name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string relationship { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime date_of_birth { get; set; }

    [StringLength(15)]
    public string? mobile_number { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal share_percentage { get; set; }

    [ForeignKey(nameof(customer_id))]
    public Customer? customer { get; set; }
}
