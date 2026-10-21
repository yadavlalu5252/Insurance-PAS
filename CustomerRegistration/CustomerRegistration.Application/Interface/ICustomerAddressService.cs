
using CustomerRegistration.Application.DTOs.Address;

namespace CustomerRegistration.Application.Interfaces;

public interface ICustomerAddressService
{
    Task<CustomerAddressResponseDto?> AddAddress(long customerId, CreateCustomerAddressDto dto);
    Task<List<CustomerAddressResponseDto>> GetAddresses(long customerId);
    Task<CustomerAddressResponseDto?> UpdateAddress(long addressId, UpdateCustomerAddressDto dto);
    Task<bool> DeleteAddress(long addressId);
}
