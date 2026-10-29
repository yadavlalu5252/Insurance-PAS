using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Api.Wrappers;
using PolicyServicing.Infrastructure.Repository;

namespace PolicyServicing.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly OtpService otp;

        public OtpController(OtpService otp)
        {
            this.otp = otp;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                return BadRequest(ApiResponse<object>.ErrorResponse("Email is required."));

            await otp.SendAsync(newEmail);
            return Ok(ApiResponse<object>.SuccessResponse(
            new { email = newEmail },
            "OTP sent successfully."));

        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(string email, string code)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
                return BadRequest(ApiResponse<object>.ErrorResponse("Email and code are required."));

            var verified = await otp.VerifyAsync(email, code);

            if (!verified)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OTP."));

            return Ok(ApiResponse<object>.SuccessResponse(
                new { email },
                "OTP verified successfully."));


        }
    }
}
