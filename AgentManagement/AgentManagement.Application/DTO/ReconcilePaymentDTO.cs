using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Application.DTO
{
    public class ReconcilePaymentDTO
    {
        public string TransactionReference { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
