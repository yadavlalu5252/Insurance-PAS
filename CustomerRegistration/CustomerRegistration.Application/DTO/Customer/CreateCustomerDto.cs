
using System.ComponentModel.DataAnnotations;

namespace CustomerRegistration.Application.DTOs.Customer;

public class CreateCustomerDto
{
    [Required, StringLength(100)]
    public string first_name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? middle_name { get; set; }

    [Required, StringLength(100)]
    public string last_name { get; set; } = string.Empty;

    [Required]
    public DateTime date_of_birth { get; set; }

    [Required, RegularExpression("^(Male|Female|Other)$")]
    public string gender { get; set; } = string.Empty;

    [Required, RegularExpression("^(Single|Married|Divorced|Widowed)$")]
    public string marital_status { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string occupation { get; set; } = string.Empty;

    [Range(0, 9999999999999.99)]
    public decimal annual_income { get; set; }

    [Required, Phone, StringLength(15)]
    public string mobile_number { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string email { get; set; } = string.Empty;
}
