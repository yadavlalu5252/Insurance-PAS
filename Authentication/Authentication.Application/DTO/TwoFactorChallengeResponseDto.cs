namespace Authentication.Application.DTO
{
    public class TwoFactorChallengeResponseDto
    {
        public bool requires_two_factor { get; set; }

        public string challenge_token { get; set; } = string.Empty;

        public DateTime expires_at { get; set; }
    }
}