
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsuranceProductManagement.Domain.Entities
{
    public class ProductCommission
    {
        [Key]
        public int commission_id { get; set; }

        public int product_id { get; set; }

     
        public decimal? first_year_commission { get; set; }

       
        public decimal? renewal_commission { get; set; }

      
        public decimal? branch_commission { get; set; }
    }
}
