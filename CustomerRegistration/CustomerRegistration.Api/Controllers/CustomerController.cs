
using CustomerRegistration.Api.Wrappers;
using CustomerRegistration.Application.DTO.Customer;
using CustomerRegistration.Application.DTOs.Customer;
using CustomerRegistration.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRegistration.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService service;

    public CustomerController(ICustomerService service)
    {
        this.service = service;
    }

    // Admin and Agent can create customers
    [HttpPost]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> CreateCustomer(CreateCustomerDto dto)
    {
        var result = await service.CreateCustomer(dto);

        return StatusCode(201,
            ApiResponse<CustomerResponseDto>.SuccessResponse(
                result, "Customer created successfully."));
    }

    // Only Admin and Agent can view all customers
    [HttpGet]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> GetAllCustomers()
    {
        var result = await service.GetAllCustomers();

        return Ok(
            ApiResponse<List<CustomerResponseDto>>.SuccessResponse(
                result, "Customers retrieved successfully."));
    }

    // Admin and Agent can view any customer.
    // Customer access requires ownership validation before returning data.
    [HttpGet("{id:long}")]
    [Authorize(Roles = "Admin,Agent,Customer")]
    public async Task<IActionResult> GetCustomerById(long id)
    {
        var result = await service.GetCustomerById(id);

        if (result == null)
            return NotFound(
                ApiResponse<CustomerResponseDto>.ErrorResponse(
                    "Customer not found."));

        // IMPORTANT:
        // For the Customer role, verify that this customer record belongs
        // to the logged-in user before returning the result.
        // Add this check using your actual UserId-to-CustomerId relationship.

        return Ok(
            ApiResponse<CustomerResponseDto>.SuccessResponse(
                result, "Customer retrieved successfully."));
    }

    // Admin and Agent can update customers
    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> UpdateCustomer(
        long id, UpdateCustomerDto dto)
    {
        var result = await service.UpdateCustomer(id, dto);

        if (result == null)
            return NotFound(
                ApiResponse<CustomerResponseDto>.ErrorResponse(
                    "Customer not found."));

        return Ok(
            ApiResponse<CustomerResponseDto>.SuccessResponse(
                result, "Customer updated successfully."));
    }

    // Only Admin can delete customers
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCustomer(long id)
    {
        var result = await service.DeleteCustomer(id);

        if (!result)
            return NotFound(
                ApiResponse<object>.ErrorResponse(
                    "Customer not found."));

        return Ok(
            ApiResponse<DeleteCustomerResponseDto>.SuccessResponse(
                new DeleteCustomerResponseDto(),
                "Customer deleted successfully."));
    }

    // Admin and Agent can search customers
    [HttpGet("search")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> SearchCustomers(
        [FromQuery] CustomerSearchDto dto)
    {
        var result = await service.SearchCustomers(dto);

        return Ok(
            ApiResponse<List<CustomerResponseDto>>.SuccessResponse(
                result, "Customer search completed successfully."));
    }
}
