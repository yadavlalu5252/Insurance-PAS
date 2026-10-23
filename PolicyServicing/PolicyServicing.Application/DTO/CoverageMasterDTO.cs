using System.Text.Json.Serialization;

namespace PolicyServicing.Application.DTO
{
    public class CoverageMasterDTO
    {

        public long coverage_id { get; set; }


        public long product_id { get; set; }

      
        public string coverage_name { get; set; } = string.Empty;

 
        public decimal minimum_coverage { get; set; }

       
        public decimal maximum_coverage { get; set; }

        public int waiting_period_days { get; set; }

     
        public decimal deductible_amount { get; set; }
    }
}