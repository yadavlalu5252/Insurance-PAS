using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class RenewalDetailsResponseDTO
    {
        public long RenewalId { get; set; }
        public long PolicyId { get; set; }
        public DateOnly RenewalDate { get; set; }
        public DateOnly NewExpiryDate { get; set; }
        public decimal PremiumAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
