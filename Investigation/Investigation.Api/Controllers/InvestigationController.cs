using Investigation.Api.Wrappers;
using InvestigationService.Application.DTOs;
using InvestigationService.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace InvestigationService.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class InvestigationController : ControllerBase
    {
       IInvestigationService service;

        public InvestigationController(IInvestigationService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("claims/{id}/investigation")]
        public async Task<IActionResult> AddInvestigation(long id,CreateInvestigationDto dto)
        {
            await service.AddInvestigation(id, dto);

            return Ok(ApiResponse<object>.SuccessResponse(null,"Investigator assigned successfully"));
        }

        [HttpPut]
        [Route("investigations/{id}")]
        public async Task<IActionResult> SubmitInvestigation(long id,[FromForm] SubmitInvestigationDto dto)
        {
            await service.SubmitInvestigation(id, dto);

            return Ok(ApiResponse<object>.SuccessResponse(null,"Investigation report submitted successfully"));
        }
    }
}