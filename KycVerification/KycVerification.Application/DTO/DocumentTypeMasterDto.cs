using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.DTO
{
    public class DocumentTypeMasterDto
    {
        public long DocumentTypeId { get; set; }
        public string? DocumentName { get; set; } 
        public bool MandatoryFlag { get; set; }
        public string? Status { get; set; }
    }
}
