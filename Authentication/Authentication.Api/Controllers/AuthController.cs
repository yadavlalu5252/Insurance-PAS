
using Authentication.Application.DTO;
using Authentication.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authentication.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
        {
            var result = await authService.Register(request);

            if (!result)
            {
                return BadRequest("Registration failed. Check the username, password, and role.");
            }

            return Ok("User registered successfully.");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await authService.Login(request);

            if (result == null)
            {
                return Unauthorized("Invalid credentials or inactive account.");
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshToken(request);

            if (result == null)
            {
                return Unauthorized("Refresh token is invalid, expired, or revoked.");
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdValue, out var userId))
            {
                return Unauthorized("User ID not found in access token.");
            }

            var result = await authService.Logout(userId);

            if (!result)
            {
                return BadRequest("Logout failed.");
            }

            return Ok("All refresh tokens for this user have been revoked.");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdValue, out var userId))
            {
                return Unauthorized("User ID not found in access token.");
            }

            var result = await authService.ChangePassword(
                userId,
                request.current_password,
                request.new_password);

            if (!result)
            {
                return BadRequest("Password change failed. Verify the current password and new password.");
            }

            return Ok("Password changed successfully. Please log in again.");
        }


        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var result = await authService.ForgotPassword(request.username);

            if (!result)
            {
                return Ok("If the account exists, password reset instructions will be sent.");
            }

            return Ok("If the account exists, password reset instructions will be sent.");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var result = await authService.ResetPassword(request);

            if (!result)
            {
                return BadRequest("Reset token is invalid, expired, or already used.");
            }

            return Ok("Password reset successfully. Please log in with your new password.");
        }
    }
}
