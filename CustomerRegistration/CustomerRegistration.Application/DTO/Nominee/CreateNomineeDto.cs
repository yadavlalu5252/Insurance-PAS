
using System.ComponentModel.DataAnnotations;

namespace CustomerRegistration.Application.DTOs.Nominee;

public class CreateNomineeDto
{
    [Required, StringLength(150)]
    public string nominee_name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string relationship { get; set; } = string.Empty;

    [Required]
    public DateTime date_of_birth { get; set; }

    [StringLength(15)]
    public string? mobile_number { get; set; }

    [Range(typeof(decimal), "0.01", "100.00")]
    public decimal share_percentage { get; set; }
}
