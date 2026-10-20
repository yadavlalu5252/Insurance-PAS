using System.ComponentModel.DataAnnotations;

namespace CustomerRegistration.Domain.Entities
{
    public class Dummy
    {
        [Key]
        public int d_id { get; set; }
        public string d_name { get; set; }
    }
}
