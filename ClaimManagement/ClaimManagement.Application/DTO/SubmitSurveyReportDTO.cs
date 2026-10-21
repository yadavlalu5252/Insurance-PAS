using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ClaimManagement.Application.DTO
{
    public class SubmitSurveyReportDTO
    {
        public DateTime inspection_date { get; set; }
        public IFormFile report_file { get; set; } = null!;
    }
}
