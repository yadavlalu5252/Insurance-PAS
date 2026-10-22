using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class CancellationDetailsDTO
    {
        public long CancellationId { get; set; }
        public long PolicyId { get; set; }
        public DateOnly CancellationDate { get; set; }
        public string CancellationReason { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; }
        public long ApprovedBy { get; set; }
    }
}
