using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class SurveyAssignment
    {
        [Key]
        public long survey_id { get; set; }

        public long claim_id { get; set; }

        public long surveyor_id { get; set; }

        public DateTime assigned_date { get; set; }

        public DateTime? inspection_date { get; set; }

        public string? report_path { get; set; }

        public string survey_status { get; set; } = SurveyStatuses.Assigned;

        [ForeignKey("claim_id")]
        public ClaimMaster? claim { get; set; }
    }
}
