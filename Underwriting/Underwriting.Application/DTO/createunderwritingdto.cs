using System.ComponentModel.DataAnnotations;

namespace Underwriting.Application.DTO
{
    public class createunderwritingdto
    {
        [Required]
        public long quote_id { get; set; }
        public long customer_id { get; set; }
        public long product_id { get; set; }
    }
}
