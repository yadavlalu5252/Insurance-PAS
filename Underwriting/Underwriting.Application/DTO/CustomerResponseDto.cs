
namespace Underwriting.Application.DTO
{
    public class CustomerResponseDto
    {
        public long customer_id { get; set; }
        public DateTime date_of_birth { get; set; }
        public string occupation { get; set; } = string.Empty;
        public decimal annual_income { get; set; }
        public string customer_status { get; set; } = "Active";
    }
}
