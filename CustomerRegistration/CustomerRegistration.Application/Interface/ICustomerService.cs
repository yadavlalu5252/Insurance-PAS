
using CustomerRegistration.Application.DTOs.Customer;

namespace CustomerRegistration.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponseDto> CreateCustomer(CreateCustomerDto dto);
    Task<List<CustomerResponseDto>> GetAllCustomers();
    Task<CustomerResponseDto?> GetCustomerById(long id);
    Task<CustomerResponseDto?> UpdateCustomer(long id, UpdateCustomerDto dto);
    Task<bool> DeleteCustomer(long id);
    Task<List<CustomerResponseDto>> SearchCustomers(CustomerSearchDto dto);
}
