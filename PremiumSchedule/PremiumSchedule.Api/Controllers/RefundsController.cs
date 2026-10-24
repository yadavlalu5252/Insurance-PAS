using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;

namespace PremiumSchedule.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundsController : ControllerBase
    {
        private readonly IRefundService refundService;

        public RefundsController(
            IRefundService refundService)
        {
            this.refundService = refundService;
        }



        [HttpPost]
        public async Task<IActionResult> CreateRefund(
            [FromBody] CreateRefundDto dto)
        {
            try
            {
                var result =
                    await refundService.CreateRefundAsync(dto);

                return CreatedAtAction(
                    nameof(GetRefund),
                    new
                    {
                        refundId = result.refund_id
                    },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


       
        [HttpPut("{refundId}/approve")]
        public async Task<IActionResult> ApproveRefund(
            int refundId,
            [FromBody] ApproveRefundDto dto)
        {
            try
            {
                var result =
                    await refundService.ApproveRefundAsync(
                        refundId,
                        dto);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Refund not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        

        [HttpGet("{refundId}")]
        public async Task<IActionResult> GetRefund(
            int refundId)
        {
            var result =
                await refundService.GetRefundByIdAsync(
                    refundId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Refund not found."
                });
            }

            return Ok(result);
        }
    }
}
