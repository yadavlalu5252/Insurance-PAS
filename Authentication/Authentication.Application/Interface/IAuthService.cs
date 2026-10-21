using Authentication.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication.Application.Interface
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> Login(LoginRequestDto request);

        Task<bool> Register(RegisterUserDto request);

        Task<bool> Logout(long user_id);

        Task<LoginResponseDto?> RefreshToken(RefreshTokenRequestDto request);

        Task<bool> ChangePassword(long user_id, string current_password, string new_password);

        Task<bool> ForgotPassword(string username);

        Task<bool> ResetPassword(ResetPasswordRequestDto request);

    }
}
