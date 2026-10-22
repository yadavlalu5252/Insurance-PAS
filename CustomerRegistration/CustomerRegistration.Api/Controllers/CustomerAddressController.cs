
using CustomerRegistration.Api.Wrappers;
using CustomerRegistration.Application.DTOs.Address;
using CustomerRegistration.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRegistration.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:long}/addresses")]
[Authorize]
public class CustomerAddressController : ControllerBase
{
    private readonly ICustomerAddressService service;

    public CustomerAddressController(ICustomerAddressService service)
    {
        this.service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> AddAddress(long customerId, CreateCustomerAddressDto dto)
    {
        var result = await service.AddAddress(customerId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Customer not found."));

        return StatusCode(201, ApiResponse<CustomerAddressResponseDto>.SuccessResponse(result, "Address added successfully."));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> GetAddresses(long customerId)
    {
        var result = await service.GetAddresses(customerId);

        return Ok(ApiResponse<List<CustomerAddressResponseDto>>.SuccessResponse(result, "Addresses retrieved successfully."));
    }

    [HttpPut("/api/customer-addresses/{addressId:long}")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> UpdateAddress(long addressId, UpdateCustomerAddressDto dto)
    {
        var result = await service.UpdateAddress(addressId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Address not found."));

        return Ok(ApiResponse<CustomerAddressResponseDto>.SuccessResponse(result, "Address updated successfully."));
    }

    [HttpDelete("/api/customer-addresses/{addressId:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAddress(long addressId)
    {
        var result = await service.DeleteAddress(addressId);

        if (!result)
            return NotFound(ApiResponse<object>.ErrorResponse("Address not found."));

        return Ok(ApiResponse<object>.SuccessResponse(new { address_id = addressId }, "Address deleted successfully."));
    }
}
