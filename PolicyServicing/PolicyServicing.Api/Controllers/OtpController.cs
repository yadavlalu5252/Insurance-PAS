using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            await otp.SendAsync(newEmail);
            return Ok($"OTP sent to {newEmail}");

        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(string email, string code)
        {
            return await otp.VerifyAsync(email, code)
                ? Ok("Verified") : BadRequest("Invalid or expired OTP");
        }
    }
}
