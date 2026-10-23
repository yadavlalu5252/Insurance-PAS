using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class PremiumFrequencyResponse
    {
        public long PolicyId { get; set; }
        public string PremiumFrequency { get; set; } = string.Empty;
    }
}
