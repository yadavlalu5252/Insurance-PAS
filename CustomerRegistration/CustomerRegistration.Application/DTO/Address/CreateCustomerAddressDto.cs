
using System.ComponentModel.DataAnnotations;

namespace CustomerRegistration.Application.DTOs.Address;

public class CreateCustomerAddressDto
{
    [Required, RegularExpression("^(Permanent|Communication)$")]
    public string address_type { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string address_line1 { get; set; } = string.Empty;

    [StringLength(255)]
    public string? address_line2 { get; set; }

    [Required, StringLength(100)]
    public string city { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string district { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string state { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string country { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string pincode { get; set; } = string.Empty;
}
