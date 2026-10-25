using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class RejectClaimDTO
    {
        public string reason { get; set; } = string.Empty;
        public bool is_fraud { get; set; }
    }
}
