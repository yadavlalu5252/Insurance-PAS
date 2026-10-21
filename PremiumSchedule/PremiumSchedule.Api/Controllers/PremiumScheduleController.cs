using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremiumSchedule.Application.Common;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;
using PremiumSchedule.Domain.Entities;

namespace PremiumSchedule.Api.Controllers
{
    [Route("api/v1/premium-schedules")]
    [ApiController]
    public class PremiumScheduleController : ControllerBase
    {
        private readonly IPremiumSchedule service;

        public PremiumScheduleController(IPremiumSchedule service)
        {
            this.service = service;
        }

        [HttpPost("internal/generate")]
        public async Task<IActionResult> GeneratePremiumSchedule(
            GeneratePremiumScheduleDto dto)
        {
            await service.GeneratePremiumSchedule(dto);

            return Ok(
                new ResponseWrapper<object>(
                    true,
                    "Premium schedule generated successfully.",
                    null));
        }

        [HttpGet("{policy_id}")]
        public async Task<IActionResult> GetPremiumScheduleByPolicyId(
            int policy_id)
        {
            var schedules =
                await service.GetPremiumScheduleByPolicyId(policy_id);

            if (schedules == null || !schedules.Any())
            {
                return NotFound(
                    new ResponseWrapper<object>(
                        false,
                        "No premium schedule found for this policy.",
                        null));
            }

            return Ok(
                new ResponseWrapper<List<PremiumScheduleDto>>(
                    true,
                    "Premium schedules retrieved successfully.",
                    schedules));
        }

        [HttpPut("{schedule_id}")]
        public async Task<IActionResult> UpdatePremiumSchedule(
            int schedule_id,
            UpdatePremiumScheduleDto dto)
        {
            var schedule =
                await service.UpdatePremiumSchedule(
                    schedule_id,
                    dto);

            if (schedule == null)
            {
                return NotFound(
                    new ResponseWrapper<object>(
                        false,
                        "Premium schedule not found.",
                        null));
            }

            return Ok(
                new ResponseWrapper<PremiumScheduleDto>(
                    true,
                    "Premium schedule updated successfully.",
                    schedule));
        }

        [HttpPost("internal/update-overdue")]
        public async Task<IActionResult> UpdateOverdueSchedules(
            decimal dailyPenaltyRate,
            decimal maxLateFee)
        {
            await service.UpdateOverdueSchedules(
                dailyPenaltyRate,
                maxLateFee);

            return Ok(
                new ResponseWrapper<object>(
                    true,
                    "Overdue premium schedules updated successfully.",
                    null));
        }
    }
}