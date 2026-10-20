using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceProductManagement.Domain.Entities
{
    public class ProductCategory
    {
        [Key]
        public int category_id { get; set; }
        public string category_name { get; set; }
        public string description { get; set; }
        public string status { get; set; }
    }
}
