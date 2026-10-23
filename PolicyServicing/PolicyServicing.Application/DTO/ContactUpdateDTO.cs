using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class ContactUpdateDTO
    {
        public string MobileNumber { get; set; }
        public string AlternateMobile { get; set; }
        public string EmailAddress { get; set; }
        public string OtpCode { get; set; }
    }
}
