
using AutoMapper;
using CustomerRegistration.Application.DTOs.Nominee;
using CustomerRegistration.Application.Interfaces;
using CustomerRegistration.Domain.Entities;
using CustomerRegistration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerRegistration.Infrastructure.Services;

public class NomineeService : INomineeService
{
    private readonly AppDbContext db;
    private readonly IMapper mapper;

    public NomineeService(AppDbContext db, IMapper mapper)
    {
        this.db = db;
        this.mapper = mapper;
    }

    public async Task<NomineeResponseDto?> AddNominee(
        long customerId, CreateNomineeDto dto)
    {
        var customerExists = await db.customers
            .AnyAsync(x => x.customer_id == customerId);

        if (!customerExists)
            return null;

        var existingShare = await db.nominees
            .Where(x => x.customer_id == customerId)
            .SumAsync(x => (decimal?)x.share_percentage) ?? 0m;

        if (existingShare + dto.share_percentage > 100m)
        {
            throw new InvalidOperationException(
                "Total nominee share percentage cannot exceed 100%.");
        }

        var nominee = mapper.Map<Nominee>(dto);
        nominee.customer_id = customerId;

        db.nominees.Add(nominee);
        await db.SaveChangesAsync();

        return mapper.Map<NomineeResponseDto>(nominee);
    }

    public async Task<List<NomineeResponseDto>> GetNominees(long customerId)
    {
        var nominees = await db.nominees
            .AsNoTracking()
            .Where(x => x.customer_id == customerId)
            .ToListAsync();

        return mapper.Map<List<NomineeResponseDto>>(nominees);
    }

    public async Task<NomineeResponseDto?> UpdateNominee(
        long nomineeId, UpdateNomineeDto dto)
    {
        var nominee = await db.nominees
            .FirstOrDefaultAsync(x => x.nominee_id == nomineeId);

        if (nominee == null)
            return null;

        var existingShare = await db.nominees
            .Where(x => x.customer_id == nominee.customer_id &&
                        x.nominee_id != nomineeId)
            .SumAsync(x => (decimal?)x.share_percentage) ?? 0m;

        if (existingShare + dto.share_percentage > 100m)
        {
            throw new InvalidOperationException(
                "Total nominee share percentage cannot exceed 100%.");
        }

        mapper.Map(dto, nominee);

        await db.SaveChangesAsync();

        return mapper.Map<NomineeResponseDto>(nominee);
    }

    public async Task<bool> DeleteNominee(long nomineeId)
    {
        var nominee = await db.nominees
            .FirstOrDefaultAsync(x => x.nominee_id == nomineeId);

        if (nominee == null)
            return false;

        db.nominees.Remove(nominee);
        await db.SaveChangesAsync();

        return true;
    }
}
