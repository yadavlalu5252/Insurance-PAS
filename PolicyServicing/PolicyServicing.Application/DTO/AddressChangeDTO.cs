using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class AddressChangeDTO
    {
        public string AddressType { get; set; } 

        public string HouseNo { get; set; }      
        public string Street { get; set; }       
        public string City { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }

        
        public string AddressProofDocumentPath { get; set; }
    }
}
