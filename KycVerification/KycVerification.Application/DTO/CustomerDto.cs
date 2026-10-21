using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.DTO
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string? CustomerCode { get; set; } 
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 
        public DateOnly DateOfBirth { get; set; }
        public string? MobileNumber { get; set; } 
        public string? Email { get; set; } 
        public string? CustomerStatus { get; set; } 
    }
}
