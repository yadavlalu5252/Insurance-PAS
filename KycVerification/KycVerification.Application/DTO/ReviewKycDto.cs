using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.DTO
{
    public class ReviewKycDto
    {
        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
