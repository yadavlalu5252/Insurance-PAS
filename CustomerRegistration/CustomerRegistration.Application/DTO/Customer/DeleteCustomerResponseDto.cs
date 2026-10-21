namespace CustomerRegistration.Application.DTO.Customer;

public class DeleteCustomerResponseDto
{
    public long customer_id { get; set; }
    public string message { get; set; } = string.Empty;
}
