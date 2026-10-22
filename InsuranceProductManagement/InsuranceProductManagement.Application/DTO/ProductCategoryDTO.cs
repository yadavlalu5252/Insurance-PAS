using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceProductManagement.Application.DTO
{
    public class ProductCategoryDTO
    {
        public int category_id { get; set; }

        [Required]
        [StringLength(100)]
        public string? category_name { get; set; }

        [StringLength(255)]
        public string? description { get; set; }

        public string? status { get; set; }
    }
}
