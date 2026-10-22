using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class SurveyResponseDTO
    {
        public long survey_id { get; set; }
        public long claim_id { get; set; }
        public long surveyor_id { get; set; }
        public DateTime assigned_date { get; set; }
        public DateTime? inspection_date { get; set; }
        public string survey_status { get; set; } = string.Empty;
    }
}
