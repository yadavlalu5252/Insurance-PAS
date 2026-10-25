
namespace Authentication.Application.DTO
{
    public class LoginResultDto
    {
        public bool requires_two_factor { get; set; }

        public string? challenge_token { get; set; }

        public DateTime? challenge_expires_at { get; set; }

        public LoginResponseDto? login { get; set; }
    }
}
