
using CustomerRegistration.Api.Wrappers;
using CustomerRegistration.Application.DTOs.Nominee;
using CustomerRegistration.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRegistration.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:long}/nominees")]
[Authorize]
public class NomineeController : ControllerBase
{
    private readonly INomineeService service;

    public NomineeController(INomineeService service)
    {
        this.service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> AddNominee(long customerId, CreateNomineeDto dto)
    {
        var result = await service.AddNominee(customerId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Customer not found."));

        return StatusCode(201, ApiResponse<NomineeResponseDto>.SuccessResponse(result, "Nominee added successfully."));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> GetNominees(long customerId)
    {
        var result = await service.GetNominees(customerId);

        return Ok(ApiResponse<List<NomineeResponseDto>>.SuccessResponse(result, "Nominees retrieved successfully."));
    }

    [HttpPut("/api/nominees/{nomineeId:long}")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> UpdateNominee(long nomineeId, UpdateNomineeDto dto)
    {
        var result = await service.UpdateNominee(nomineeId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Nominee not found."));

        return Ok(ApiResponse<NomineeResponseDto>.SuccessResponse(result, "Nominee updated successfully."));
    }

    [HttpDelete("/api/nominees/{nomineeId:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteNominee(long nomineeId)
    {
        var result = await service.DeleteNominee(nomineeId);

        if (!result)
            return NotFound(ApiResponse<object>.ErrorResponse("Nominee not found."));

        return Ok(ApiResponse<object>.SuccessResponse(new { nominee_id = nomineeId }, "Nominee deleted successfully."));
    }
}
