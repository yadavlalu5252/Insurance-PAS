
using System.ComponentModel.DataAnnotations;

namespace CustomerRegistration.Application.DTOs.Document;

public class VerifyCustomerDocumentDto
{
    [Required]
    [RegularExpression("^(Verified|Rejected)$")]
    public string verification_status { get; set; } = string.Empty;
}
