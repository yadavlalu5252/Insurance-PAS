
namespace CustomerRegistration.Application.DTOs.Contact;

public class CustomerContactResponseDto
{
    public long contact_id { get; set; }
    public long customer_id { get; set; }
    public string? alternate_mobile { get; set; }
    public string? office_phone { get; set; }
    public string preferred_contact_mode { get; set; } = "Mobile";
}
    