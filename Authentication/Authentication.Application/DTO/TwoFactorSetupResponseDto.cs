namespace Authentication.Application.DTO
{
    public class TwoFactorSetupResponseDto
    {
        public string shared_key { get; set; } = string.Empty;

        public string otpauth_url { get; set; } = string.Empty;
    }
}