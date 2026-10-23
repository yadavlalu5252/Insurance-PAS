
namespace QuoteGeneration.Application.DTO
{
    public class RiderMasterDTO
    {
        public int rider_id { get; set; }
        public int product_id { get; set; }
        public string rider_name { get; set; } = string.Empty;
        public string rider_description { get; set; } = string.Empty;
        public decimal rider_premium { get; set; }
        public string status { get; set; } = string.Empty;
    }
}
