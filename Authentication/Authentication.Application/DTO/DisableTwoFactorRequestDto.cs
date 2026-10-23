
using System.ComponentModel.DataAnnotations;

namespace Authentication.Application.DTO
{
    public class DisableTwoFactorRequestDto
    {
        [Required]
        public string current_password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string code { get; set; } = string.Empty;
    }
}
