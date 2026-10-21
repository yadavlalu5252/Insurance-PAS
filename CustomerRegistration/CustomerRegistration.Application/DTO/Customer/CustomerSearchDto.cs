
using System.ComponentModel.DataAnnotations;

namespace CustomerRegistration.Application.DTOs.Customer;

public class CustomerSearchDto
{
    public long? customer_id { get; set; }

    [StringLength(15)]
    public string? mobile_number { get; set; }

    [EmailAddress, StringLength(150)]
    public string? email { get; set; }

    [StringLength(20)]
    public string? customer_code { get; set; }

    [StringLength(100)]
    public string? customer_name { get; set; }

    [StringLength(20)]
    public string? customer_status { get; set; }

    public string? aadhaar_number { get; set; }

    public string? pan_number { get; set; }

    public string? policy_number { get; set; }
}
