
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


        [Authorize]
        [HttpPost("2fa/setup")]
        public async Task<IActionResult> SetupTwoFactor()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdValue, out var userId))
                return Unauthorized();

            try
            {
                var result = await authService.SetupTwoFactor(userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> EnableTwoFactor(
            [FromBody] TwoFactorSetupVerifyDto request)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdValue, out var userId))
                return Unauthorized();

            var result = await authService.EnableTwoFactor(userId, request);

            if (!result)
                return BadRequest("Invalid authenticator code or setup.");

            return Ok("Two-factor authentication enabled successfully.");
        }

        [Authorize]
        [HttpPost("2fa/disable")]
        public async Task<IActionResult> DisableTwoFactor(
            [FromBody] DisableTwoFactorRequestDto request)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdValue, out var userId))
                return Unauthorized();

            var result = await authService.DisableTwoFactor(
                userId,
                request.current_password,
                request.code);

            if (!result)
                return BadRequest("Password or authenticator code is invalid.");

            return Ok("Two-factor authentication disabled successfully.");
        }

        [AllowAnonymous]
        [HttpPost("2fa/verify-login")]
        public async Task<IActionResult> VerifyTwoFactorLogin(
            [FromBody] TwoFactorVerifyDto request)
        {
            var result = await authService.VerifyTwoFactorLogin(request);

            if (result == null)
                return Unauthorized("Invalid, expired, or used 2FA challenge.");

            return Ok(result);
        }


    }
}
