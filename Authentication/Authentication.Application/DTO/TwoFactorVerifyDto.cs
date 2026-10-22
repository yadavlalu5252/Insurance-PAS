using System.ComponentModel.DataAnnotations;

namespace Authentication.Application.DTO
{
    public class TwoFactorVerifyDto
    {
        [Required]
        public string challenge_token { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "The code must contain exactly 6 digits.")]
        public string code { get; set; } = string.Empty;
    }
}