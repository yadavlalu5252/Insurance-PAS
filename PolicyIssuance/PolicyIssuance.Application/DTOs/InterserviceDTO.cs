
namespace PolicyIssuance.Application.DTOs
{
    public class CustomerServiceDTO
    {
        public long customer_id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string customer_status { get; set; } = string.Empty;
    }

    public class KycServiceDTO
    {
        public int kyc_id { get; set; }
        public int customer_id { get; set; }
        public string verification_status { get; set; } = string.Empty;
    }

    public class QuoteServiceDTO
    {
        public int quote_id { get; set; }
        public int customer_id { get; set; }
        public int product_id { get; set; }
        public decimal sum_insured { get; set; }
        public string? quote_status { get; set; }
    }

    public class UnderwritingServiceDTO
    {
        public long underwriting_id { get; set; }
        public long quote_id { get; set; }
        public long customer_id { get; set; }
        public long product_id { get; set; }
    }

    public class ProductServiceDTO
    {
        public long product_id { get; set; }
        public string product_name { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
    }

    public class ServiceResponseDTO<T>
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public T? data { get; set; }
    }
}
