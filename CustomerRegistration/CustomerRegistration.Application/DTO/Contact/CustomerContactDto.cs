using System.ComponentModel.DataAnnotations;

namespace CustomerRegistration.Application.DTO.Contact;

public class CustomerContactDto
{
    [StringLength(15)]
    public string? alternate_mobile { get; set; }

    [StringLength(20)]
    public string? office_phone { get; set; }

    [Required, RegularExpression("^(Mobile|Email|SMS)$")]
    public string preferred_contact_mode { get; set; } = "Mobile";
}
