
using Authentication.Application.DTO;

namespace Authentication.Application.Interface
{
    public interface IAuthService
    {
        Task<LoginResultDto?> Login(LoginRequestDto request);

        Task<bool> Register(RegisterUserDto request);

        Task<bool> Logout(long user_id);

        Task<LoginResponseDto?> RefreshToken(RefreshTokenRequestDto request);

        Task<bool> ChangePassword(long user_id, string current_password, string new_password);

        Task<bool> ForgotPassword(string username);

        Task<bool> ResetPassword(ResetPasswordRequestDto request);

        // Two-factor authentication
        Task<TwoFactorSetupResponseDto> SetupTwoFactor(long user_id);

        Task<bool> EnableTwoFactor(long user_id, TwoFactorSetupVerifyDto request);

        Task<bool> DisableTwoFactor(long user_id, string current_password, string code);

        Task<LoginResponseDto?> VerifyTwoFactorLogin(TwoFactorVerifyDto request);
    }
}
