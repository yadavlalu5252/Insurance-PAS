using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Domain.Entities
{
    public class DocumentTypeMaster
    {
        [Key]
        public int document_type_id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? document_name { get; set; } 

        public bool mandatory_flag { get; set; }

        [Required]
        [MaxLength(20)]
        public string status { get; set; } = "Active";
    }
}
