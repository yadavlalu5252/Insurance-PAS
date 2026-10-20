
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceProductManagement.Domain.Entities
{
    public class RiderMaster
    {
        [Key]
        public int rider_id { get; set; }

        public int product_id { get; set; }

        [StringLength(150)]
        public string? rider_name { get; set; } 

        public string ?  rider_description { get; set; } 

        public decimal? rider_premium { get; set; }

        
        public string status { get; set; }
    }
}
