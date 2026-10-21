
using CustomerRegistration.Api.Wrappers;
using CustomerRegistration.Application.DTOs.Nominee;
using CustomerRegistration.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRegistration.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:long}/nominees")]
public class NomineeController : ControllerBase
{
    private readonly INomineeService _nomineeService;

    public NomineeController(INomineeService nomineeService)
    {
        _nomineeService = nomineeService;
    }

    [HttpPost]
    public async Task<IActionResult> AddNominee(
        long customerId, CreateNomineeDto dto)
    {
        var result = await _nomineeService.AddNominee(customerId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse(
                "Customer not found."));

        return StatusCode(201,
            ApiResponse<NomineeResponseDto>.SuccessResponse(
                result, "Nominee added successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetNominees(long customerId)
    {
        var result = await _nomineeService.GetNominees(customerId);

        return Ok(
            ApiResponse<List<NomineeResponseDto>>.SuccessResponse(
                result, "Nominees retrieved successfully."));
    }

    [HttpPut("/api/nominees/{nomineeId:long}")]
    public async Task<IActionResult> UpdateNominee(
        long nomineeId, UpdateNomineeDto dto)
    {
        var result = await _nomineeService.UpdateNominee(nomineeId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse(
                "Nominee not found."));

        return Ok(
            ApiResponse<NomineeResponseDto>.SuccessResponse(
                result, "Nominee updated successfully."));
    }

    [HttpDelete("/api/nominees/{nomineeId:long}")]
    public async Task<IActionResult> DeleteNominee(long nomineeId)
    {
        var result = await _nomineeService.DeleteNominee(nomineeId);

        if (!result)
            return NotFound(ApiResponse<object>.ErrorResponse(
                "Nominee not found."));

        return Ok(
            ApiResponse<object>.SuccessResponse(
                new { nominee_id = nomineeId },
                "Nominee deleted successfully."));
    }
}
