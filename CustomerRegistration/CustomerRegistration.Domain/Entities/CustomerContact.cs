
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerRegistration.Domain.Entities;

[Table("customer_contact")]
public class CustomerContact
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long contact_id { get; set; }

    public long customer_id { get; set; }

    [StringLength(15)]
    public string? alternate_mobile { get; set; }

    [StringLength(20)]
    public string? office_phone { get; set; }

    [Required]
    [StringLength(10)]
    public string preferred_contact_mode { get; set; } = "Mobile";

    [ForeignKey(nameof(customer_id))]
    public Customer? customer { get; set; }
}
