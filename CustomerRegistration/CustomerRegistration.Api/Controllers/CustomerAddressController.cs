
using CustomerRegistration.Api.Wrappers;
using CustomerRegistration.Application.DTOs.Address;
using CustomerRegistration.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRegistration.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:long}/addresses")]
public class CustomerAddressController : ControllerBase
{
    private readonly ICustomerAddressService _addressService;

    public CustomerAddressController(ICustomerAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress(
        long customerId, CreateCustomerAddressDto dto)
    {
        var result = await _addressService.AddAddress(customerId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse(
                "Customer not found."));

        return StatusCode(201,
            ApiResponse<CustomerAddressResponseDto>.SuccessResponse(
                result, "Address added successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses(long customerId)
    {
        var result = await _addressService.GetAddresses(customerId);

        return Ok(
            ApiResponse<List<CustomerAddressResponseDto>>.SuccessResponse(
                result, "Addresses retrieved successfully."));
    }

    [HttpPut("/api/customer-addresses/{addressId:long}")]
    public async Task<IActionResult> UpdateAddress(
        long addressId, UpdateCustomerAddressDto dto)
    {
        var result = await _addressService.UpdateAddress(addressId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse(
                "Address not found."));

        return Ok(
            ApiResponse<CustomerAddressResponseDto>.SuccessResponse(
                result, "Address updated successfully."));
    }

    [HttpDelete("/api/customer-addresses/{addressId:long}")]
    public async Task<IActionResult> DeleteAddress(long addressId)
    {
        var result = await _addressService.DeleteAddress(addressId);

        if (!result)
            return NotFound(ApiResponse<object>.ErrorResponse(
                "Address not found."));

        return Ok(
            ApiResponse<object>.SuccessResponse(
                new { address_id = addressId },
                "Address deleted successfully."));
    }
}
