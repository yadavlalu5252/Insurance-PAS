
using AutoMapper;
using CustomerRegistration.Application.DTOs.Customer;
using CustomerRegistration.Application.Interfaces;
using CustomerRegistration.Domain.Entities;
using CustomerRegistration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerRegistration.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext db;
    private readonly IMapper mapper;

    public CustomerService(AppDbContext db, IMapper mapper)
    {
        this.db = db;
        this.mapper = mapper;
    }

    public async Task<CustomerResponseDto> CreateCustomer(CreateCustomerDto dto)
    {
        if (await db.customers.AnyAsync(x =>
            x.mobile_number == dto.mobile_number || x.email == dto.email))
        {
            throw new InvalidOperationException(
                "Customer with this mobile number or email already exists.");
        }

        var customer = mapper.Map<Customer>(dto);

        customer.customer_code = "CUS" + Guid.NewGuid().ToString("N")[..17];
        customer.customer_status = "Active";
        customer.created_at = DateTime.UtcNow;
        customer.updated_at = DateTime.UtcNow;

        db.customers.Add(customer);
        await db.SaveChangesAsync();

        return mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<List<CustomerResponseDto>> GetAllCustomers()
    {
        var customers = await db.customers
            .AsNoTracking()
            .ToListAsync();

        return mapper.Map<List<CustomerResponseDto>>(customers);
    }

    public async Task<CustomerResponseDto?> GetCustomerById(long id)
    {
        var customer = await db.customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.customer_id == id);

        return customer == null
            ? null
            : mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<CustomerResponseDto?> UpdateCustomer(
        long id, UpdateCustomerDto dto)
    {
        var customer = await db.customers
            .FirstOrDefaultAsync(x => x.customer_id == id);

        if (customer == null)
            return null;

        if (await db.customers.AnyAsync(x =>
            x.customer_id != id &&
            (x.mobile_number == dto.mobile_number || x.email == dto.email)))
        {
            throw new InvalidOperationException(
                "Another customer already uses this mobile number or email.");
        }

        mapper.Map(dto, customer);
        customer.updated_at = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<bool> DeleteCustomer(long id)
    {
        var customer = await db.customers
            .FirstOrDefaultAsync(x => x.customer_id == id);

        if (customer == null)
            return false;

        db.customers.Remove(customer);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<List<CustomerResponseDto>> SearchCustomers(
        CustomerSearchDto dto)
    {
        var query = db.customers.AsNoTracking().AsQueryable();

        if (dto.customer_id.HasValue)
            query = query.Where(x => x.customer_id == dto.customer_id.Value);

        if (!string.IsNullOrWhiteSpace(dto.customer_code))
            query = query.Where(x => x.customer_code.Contains(dto.customer_code));

        if (!string.IsNullOrWhiteSpace(dto.mobile_number))
            query = query.Where(x => x.mobile_number.Contains(dto.mobile_number));

        if (!string.IsNullOrWhiteSpace(dto.email))
            query = query.Where(x => x.email.Contains(dto.email));

        if (!string.IsNullOrWhiteSpace(dto.customer_name))
        {
            var name = dto.customer_name.Trim();

            query = query.Where(x =>
                x.first_name.Contains(name) ||
                x.last_name.Contains(name) ||
                (x.middle_name != null && x.middle_name.Contains(name)));
        }

        if (!string.IsNullOrWhiteSpace(dto.customer_status))
            query = query.Where(x => x.customer_status == dto.customer_status);

        var customers = await query.ToListAsync();

        return mapper.Map<List<CustomerResponseDto>>(customers);
    }
}
