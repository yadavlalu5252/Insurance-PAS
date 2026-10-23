using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class ApproveRefundDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int approved_by { get; set; }
    }
}
