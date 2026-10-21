using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class ClaimDocument
    {
        [Key]
        public long document_id { get; set; }

        public long claim_id { get; set; }

        public string document_name { get; set; } = string.Empty;

        public string file_path { get; set; } = string.Empty;

        public string verification_status { get; set; } = "Pending";

        public DateTime uploaded_date { get; set; }
    }
}
