
namespace CustomerRegistration.Application.DTOs.Nominee;

public class NomineeResponseDto
{
    public long nominee_id { get; set; }
    public long customer_id { get; set; }
    public string nominee_name { get; set; } = string.Empty;
    public string relationship { get; set; } = string.Empty;
    public DateTime date_of_birth { get; set; }
    public string? mobile_number { get; set; }
    public decimal share_percentage { get; set; }
}
