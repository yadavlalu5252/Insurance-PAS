
namespace CustomerRegistration.Application.DTOs.Customer;

public class CustomerResponseDto
{
    public long customer_id { get; set; }
    public string customer_code { get; set; } = string.Empty;
    public string first_name { get; set; } = string.Empty;
    public string? middle_name { get; set; }
    public string last_name { get; set; } = string.Empty;
    public DateTime date_of_birth { get; set; }
    public string gender { get; set; } = string.Empty;
    public string marital_status { get; set; } = string.Empty;
    public string occupation { get; set; } = string.Empty;
    public decimal annual_income { get; set; }
    public string mobile_number { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string customer_status { get; set; } = "Active";
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
