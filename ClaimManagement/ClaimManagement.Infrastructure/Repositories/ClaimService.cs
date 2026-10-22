using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClaimManagement.Infrastructure.Repositories;

public class ClaimService : IClaimService
{
    ClaimDbContext db;
    IMapper mapper;

    // recommended list based on the BRD product types (not an official BRD dropdown)
    static readonly string[] AllowedClaimTypes =
    {
        "Health", "Motor", "Life", "Travel", "Home", "Property", "Personal Accident"
    };

    public ClaimService(ClaimDbContext db, IMapper mapper)
    {
        this.db = db;
        this.mapper = mapper;
    }

    public async Task<ClaimResponseDTO> AddClaimAsync(CreateClaimDTO dto, long performedBy)
    {
        if (dto.policy_id <= 0)
            throw new BadRequestException("Policy id is required");
        if (dto.customer_id <= 0)
            throw new BadRequestException("Customer id is required");
        if (!AllowedClaimTypes.Contains(dto.claim_type))
            throw new BadRequestException("Claim type is invalid. Allowed: " + string.Join(", ", AllowedClaimTypes));
        if (dto.incident_date == default)
            throw new BadRequestException("Incident date is required");
        if (dto.incident_date.Date > DateTime.Now.Date)
            throw new BadRequestException("Incident date cannot be in the future");
        if (string.IsNullOrWhiteSpace(dto.description))
            throw new BadRequestException("Description is required");
        if (dto.estimated_loss <= 0)
            throw new BadRequestException("Estimated loss must be greater than 0");

        // TODO: policy and premium eligibility check goes here.
        // It is NOT done yet, so claims are saved WITHOUT policy validation.

        // up to 3 tries, in case two claims get the same number at the same moment
        for (int attempt = 1; attempt <= 3; attempt++)
        {
            var claim = mapper.Map<ClaimMaster>(dto);
            claim.claim_number = await GenerateClaimNumberAsync();
            claim.claim_date = DateTime.Now;
            claim.claim_status = ClaimStatuses.Registered;
            claim.claim_history.Add(CreateHistory("Claim Registered", performedBy));

            db.claim_master.Add(claim);

            try
            {
                await db.SaveChangesAsync();
                return mapper.Map<ClaimResponseDTO>(claim);
            }
            catch (DbUpdateException ex) when (IsDuplicateKey(ex) && attempt < 3)
            {
                db.ChangeTracker.Clear();   // forget the failed claim and retry with a new number
            }
        }

        throw new InvalidOperationException("Could not generate a unique claim number");
    }

    public async Task<PagedResultDTO<ClaimResponseDTO>> FetchClaimsAsync(ClaimFilterDTO filter)
    {
        if (filter.page_number < 1)
            throw new BadRequestException("Page number must be 1 or more");
        if (filter.page_size < 1 || filter.page_size > 100)
            throw new BadRequestException("Page size must be between 1 and 100");

        var query = db.claim_master.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.search))
            query = query.Where(c => c.claim_number.Contains(filter.search));
        if (!string.IsNullOrWhiteSpace(filter.claim_status))
            query = query.Where(c => c.claim_status == filter.claim_status);
        if (!string.IsNullOrWhiteSpace(filter.claim_type))
            query = query.Where(c => c.claim_type == filter.claim_type);
        if (filter.customer_id.HasValue)
            query = query.Where(c => c.customer_id == filter.customer_id.Value);
        if (filter.policy_id.HasValue)
            query = query.Where(c => c.policy_id == filter.policy_id.Value);
        if (filter.from_date.HasValue)
        {
            var from = filter.from_date.Value.Date;
            query = query.Where(c => c.claim_date >= from);
        }
        if (filter.to_date.HasValue)
        {
            var to = filter.to_date.Value.Date.AddDays(1);
            query = query.Where(c => c.claim_date < to);
        }

        int total = await query.CountAsync();

        var data = await query
            .OrderByDescending(c => c.claim_id)
            .Skip((filter.page_number - 1) * filter.page_size)
            .Take(filter.page_size)
            .ToListAsync();

        return new PagedResultDTO<ClaimResponseDTO>
        {
            items = mapper.Map<List<ClaimResponseDTO>>(data),
            page_number = filter.page_number,
            page_size = filter.page_size,
            total_records = total,
            total_pages = (int)Math.Ceiling(total / (double)filter.page_size)
        };
    }

    public async Task<ClaimResponseDTO> FetchClaimByIdAsync(long id)
    {
        var claim = await GetClaimOrThrowAsync(id);
        return mapper.Map<ClaimResponseDTO>(claim);
    }

    public async Task<ClaimStatusDTO> FetchClaimStatusAsync(long id)
    {
        var claim = await GetClaimOrThrowAsync(id);
        return mapper.Map<ClaimStatusDTO>(claim);
    }

    public async Task<List<ClaimHistoryDTO>> FetchClaimHistoryAsync(long id)
    {
        await GetClaimOrThrowAsync(id);

        var data = await db.claim_history
            .Where(h => h.claim_id == id)
            .OrderByDescending(h => h.action_date)
            .ThenByDescending(h => h.history_id)
            .ToListAsync();

        return mapper.Map<List<ClaimHistoryDTO>>(data);
    }

    public async Task UpdateClaimAsync(long id, UpdateClaimDTO dto, long performedBy)
    {
        if (string.IsNullOrWhiteSpace(dto.description))
            throw new BadRequestException("Description is required");
        if (dto.estimated_loss <= 0)
            throw new BadRequestException("Estimated loss must be greater than 0");

        var claim = await GetClaimOrThrowAsync(id);

        // Recommendation (not in BRD): edit only before processing starts
        if (claim.claim_status != ClaimStatuses.Registered &&
            claim.claim_status != ClaimStatuses.PendingVerification)
            throw new ConflictException("Claim cannot be updated in its current status");

        mapper.Map(dto, claim);

        db.claim_history.Add(CreateHistory("Claim Updated", performedBy, claim.claim_id));
        await db.SaveChangesAsync();
    }

    // ---------- helper methods ----------

    private async Task<ClaimMaster> GetClaimOrThrowAsync(long id)
    {
        var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == id);
        if (claim == null)
            throw new NotFoundException("Claim not found");
        return claim;
    }

    private async Task<string> GenerateClaimNumberAsync()
    {
        string number;
        do
        {
            number = "CLM-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Random.Shared.Next(1000, 10000);
        }
        while (await db.claim_master.AnyAsync(c => c.claim_number == number));

        return number;
    }

    // 2601 and 2627 are SQL Server's "duplicate key" error numbers
    private static bool IsDuplicateKey(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
    }

    private static ClaimHistory CreateHistory(string action, long performedBy, long claimId = 0)
    {
        return new ClaimHistory
        {
            claim_id = claimId,
            action = action,
            performed_by = performedBy,
            action_date = DateTime.Now
        };
    }
}