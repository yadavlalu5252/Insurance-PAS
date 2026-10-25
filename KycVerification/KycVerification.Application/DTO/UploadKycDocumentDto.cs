using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.DTO
{
    public class UploadKycDocumentDto
    {
        [Required]
        public long KycId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? DocumentType { get; set; } 

        [Required]
        [MaxLength(100)]
        public string? DocumentNumber { get; set; }

        [Required]
        public IFormFile? File { get; set; }
    }
}
