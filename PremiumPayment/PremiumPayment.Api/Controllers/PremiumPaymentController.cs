using Microsoft.AspNetCore.Mvc;
using Premium.Api.Wrappers;
using Premium.Application.DTOs;
using Premium.Application.Interfaces;

namespace Premium.Api.Controllers
{
    [ApiController]
    [Route("api/v1/payments")]
    public class PremiumPaymentController : ControllerBase
    {
        private readonly IPremiumService _service;

        public PremiumPaymentController(
            IPremiumService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> PayPremium(
            [FromBody] PremiumPaymentDto dto)
        {
            var paymentId =
                await _service.PayPremium(dto);

            return Ok(
                ApiResponse<long>.SuccessResponse(
                    paymentId,
                    "Premium payment initiated successfully."));
        }

        [HttpGet("{paymentId:long}")]
        public async Task<IActionResult> GetPayment(
            long paymentId)
        {
            var payment =
                await _service.GetPayment(paymentId);

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    payment,
                    "Payment details retrieved successfully."));
        }

        [HttpGet("history/{policyId:long}")]
        public async Task<IActionResult> GetPaymentHistory(
            long policyId)
        {
            var payments =
                await _service.GetPaymentHistory(policyId);

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    payments,
                    "Payment history retrieved successfully."));
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment(
            [FromBody] VerifyPaymentDto dto)
        {
            await _service.VerifyPayment(dto);

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    null!,
                    "Payment verified successfully."));
        }
    }
}