
namespace CustomerRegistration.Application.DTOs.Address;

public class CustomerAddressResponseDto
{
    public long address_id { get; set; }
    public long customer_id { get; set; }
    public string address_type { get; set; } = string.Empty;
    public string address_line1 { get; set; } = string.Empty;
    public string? address_line2 { get; set; }
    public string city { get; set; } = string.Empty;
    public string district { get; set; } = string.Empty;
    public string state { get; set; } = string.Empty;
    public string country { get; set; } = string.Empty;
    public string pincode { get; set; } = string.Empty;
}
