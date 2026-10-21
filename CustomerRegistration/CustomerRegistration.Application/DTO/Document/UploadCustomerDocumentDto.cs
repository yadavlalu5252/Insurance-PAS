
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CustomerRegistration.Application.DTOs.Document;

public class UploadCustomerDocumentDto
{
    [Required, StringLength(100)]
    public string document_type { get; set; } = string.Empty;

    [Required]
    public IFormFile? file { get; set; }
}
