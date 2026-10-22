using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class RiderModifyDTO
    {
        public long RiderId { get; set; }
        public string Action { get; set; } // "Add" or "Remove"
    }
}
