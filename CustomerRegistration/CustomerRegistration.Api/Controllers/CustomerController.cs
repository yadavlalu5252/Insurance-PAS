
using CustomerRegistration.Api.Wrappers;
using CustomerRegistration.Application.DTO.Customer;
using CustomerRegistration.Application.DTOs.Customer;
using CustomerRegistration.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRegistration.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService service;

    public CustomerController(ICustomerService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CreateCustomerDto dto)
    {
        var result = await service.CreateCustomer(dto);

        return StatusCode(201,
            ApiResponse<CustomerResponseDto>.SuccessResponse(
                result, "Customer created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var result = await service.GetAllCustomers();

        return Ok(
            ApiResponse<List<CustomerResponseDto>>.SuccessResponse(
                result, "Customers retrieved successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetCustomerById(long id)
    {
        var result = await service.GetCustomerById(id);

        if (result == null)
            return NotFound(ApiResponse<CustomerResponseDto>.ErrorResponse(
                "Customer not found."));

        return Ok(
            ApiResponse<CustomerResponseDto>.SuccessResponse(
                result, "Customer retrieved successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateCustomer(
        long id, UpdateCustomerDto dto)
    {
        var result = await service.UpdateCustomer(id, dto);

        if (result == null)
            return NotFound(ApiResponse<CustomerResponseDto>.ErrorResponse(
                "Customer not found."));

        return Ok(
            ApiResponse<CustomerResponseDto>.SuccessResponse(
                result, "Customer updated successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteCustomer(long id)
    {
        var result = await service.DeleteCustomer(id);

        if (!result)
            return NotFound(ApiResponse<object>.ErrorResponse(
                "Customer not found."));

        return Ok(
            ApiResponse<DeleteCustomerResponseDto>.SuccessResponse(
                new DeleteCustomerResponseDto(),
                "Customer deleted successfully."));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers(
        [FromQuery] CustomerSearchDto dto)
    {
        var result = await service.SearchCustomers(dto);

        return Ok(
            ApiResponse<List<CustomerResponseDto>>.SuccessResponse(
                result, "Customer search completed successfully."));
    }
}
