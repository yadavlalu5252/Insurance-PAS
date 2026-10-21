
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerRegistration.Domain.Entities;


public class Customer
{
    [Key]
    public long customer_id { get; set; }

    [Required]
    [StringLength(20)]
    public string customer_code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string first_name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? middle_name { get; set; }

    [Required]
    [StringLength(100)]
    public string last_name { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime date_of_birth { get; set; }

    [Required]
    [StringLength(10)]
    public string gender { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string marital_status { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string occupation { get; set; } = string.Empty;

    [Column(TypeName = "decimal(15,2)")]
    public decimal annual_income { get; set; }

    [Required]
    [StringLength(15)]
    public string mobile_number { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string email { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string customer_status { get; set; } = "Active";

    public DateTime created_at { get; set; } = DateTime.UtcNow;

    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    public ICollection<CustomerAddress> addresses { get; set; } = new List<CustomerAddress>();

    public CustomerContact? contact { get; set; }

    public ICollection<Nominee> nominees { get; set; } = new List<Nominee>();

    public ICollection<CustomerDocument> documents { get; set; } = new List<CustomerDocument>();
}
