
using AutoMapper;
using CustomerRegistration.Application.DTOs.Address;
using CustomerRegistration.Application.Interfaces;
using CustomerRegistration.Application.Mapper;
using CustomerRegistration.Domain.Entities;
using CustomerRegistration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerRegistration.Infrastructure.Services;

public class CustomerAddressService : ICustomerAddressService
{
    private readonly AppDbContext db;
    private readonly IMapper mapper;

    public CustomerAddressService(AppDbContext db, IMapper mapper)
    {
        this.db = db;
        this.mapper = mapper;
    }

    public async Task<CustomerAddressResponseDto?> AddAddress(
        long customerId, CreateCustomerAddressDto dto)
    {
        var customerExists = await db.customers
            .AnyAsync(x => x.customer_id == customerId);

        if (!customerExists)
            return null;

        var address = mapper.Map<CustomerAddress>(dto);
        address.customer_id = customerId;

        db.customer_addresses.Add(address);
        await db.SaveChangesAsync();

        return mapper.Map<CustomerAddressResponseDto>(address);
    }

    public async Task<List<CustomerAddressResponseDto>> GetAddresses(
        long customerId)
    {
        var addresses = await db.customer_addresses
            .AsNoTracking()
            .Where(x => x.customer_id == customerId)
            .ToListAsync();

        return mapper.Map<List<CustomerAddressResponseDto>>(addresses);
    }

    public async Task<CustomerAddressResponseDto?> UpdateAddress(
        long addressId, UpdateCustomerAddressDto dto)
    {
        var address = await db.customer_addresses
            .FirstOrDefaultAsync(x => x.address_id == addressId);

        if (address == null)
            return null;

        mapper.Map(dto, address);

        await db.SaveChangesAsync();

        return mapper.Map<CustomerAddressResponseDto>(address);
    }

    public async Task<bool> DeleteAddress(long addressId)
    {
        var address = await db.customer_addresses
            .FirstOrDefaultAsync(x => x.address_id == addressId);

        if (address == null)
            return false;

        db.customer_addresses.Remove(address);
        await db.SaveChangesAsync();

        return true;
    }
}
