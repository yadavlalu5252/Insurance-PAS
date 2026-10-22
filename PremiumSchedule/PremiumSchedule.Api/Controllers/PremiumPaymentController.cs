using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Premium.Application.DTOs;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interface;

namespace PremiumSchedule.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PremiumPaymentController : ControllerBase
    {
        private readonly IPremiumPaymentService paymentService;

        public PremiumPaymentController(
            IPremiumPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        
        // Create Premium Payment
        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromBody] CreatePaymentDto dto)
        {
            var result = await paymentService.CreatePaymentAsync(dto);

            return Ok(result);
        }

        // Get Payment By ID
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(
            int paymentId)
        {
            var result =
                await paymentService.GetPaymentByIdAsync(paymentId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(result);
        }

        // Get All Payments
        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var result = await paymentService.GetPaymentsAsync();

            return Ok(result);
        }

        
        // Verify Payment
        [HttpPost("{paymentId}/verify")]
        public async Task<IActionResult> VerifyPayment(
            int paymentId,
            [FromBody] VerifyPaymentDto dto)
        {
            var result =
                await paymentService.VerifyPaymentAsync(
                    paymentId,
                    dto);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(result);
        }

        
        // Retry Failed Payment
        [HttpPost("{paymentId}/retry")]
        public async Task<IActionResult> RetryPayment(
            int paymentId)
        {
            var result =
                await paymentService.RetryPaymentAsync(paymentId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(result);
        }

        // Update Payment Status
        [HttpPut("{paymentId}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(
            int paymentId,
            [FromBody] PaymentStatusDto dto)
        {
            var result =
                await paymentService.UpdatePaymentStatusAsync(
                    paymentId,
                    dto);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(result);
        }





        [HttpGet("{paymentId}/receipt")]
        public async Task<IActionResult> GetReceipt(int paymentId)
        {
            var result =
                await paymentService.GetReceiptAsync(paymentId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(result);
        }


        [HttpGet("{paymentId}/receipt/download")]
        public async Task<IActionResult> DownloadReceipt(int paymentId)
        {
            var result =
                await paymentService.DownloadReceiptAsync(paymentId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Receipt not found."
                });
            }

            return File(
                result.Value.FileBytes,
                "application/pdf",
                result.Value.FileName);
        }


        // POST /api/v1/payments/{paymentId}/receipt/regenerate
        [HttpPost("{paymentId}/receipt/regenerate")]
            public async Task<IActionResult> RegenerateReceipt(
                int paymentId)
            {
                var result =
                    await paymentService.RegenerateReceiptAsync(paymentId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Payment not found."
                    });
                }

                return Ok(result);
            }

        [HttpGet("history/{policyId:int}")]
        public async Task<IActionResult> GetPaymentHistory(int policyId)
        {
            if (policyId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid policy ID."
                });
            }

            var result = await paymentService.GetPaymentHistoryByPolicyAsync(policyId);

            return Ok(result);
        }


    }
}
