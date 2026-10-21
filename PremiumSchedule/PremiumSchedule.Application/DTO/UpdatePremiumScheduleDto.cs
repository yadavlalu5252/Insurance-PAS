using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class UpdatePremiumScheduleDto
    {
        public decimal late_fee { get; set; }

        public string status { get; set; }
    }
}
