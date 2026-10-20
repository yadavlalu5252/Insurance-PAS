using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PremiumSchedule.Application.Common;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;
using PremiumSchedule.Domain.Entities;

namespace PremiumSchedule.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PremiumScheduleController : ControllerBase
    {
        IPremiumSchedule service;

        public PremiumScheduleController(IPremiumSchedule service)
        {
            this.service = service;
        }


        [HttpGet("policy/{policy_id}")]
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


        [HttpGet("{schedule_id}")]
        public async Task<IActionResult> GetPremiumScheduleById(
            int schedule_id)
        {
            var schedule =
                await service.GetPremiumScheduleById(schedule_id);

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
                    "Premium schedule retrieved successfully.",
                    schedule));
        }
    }
}
