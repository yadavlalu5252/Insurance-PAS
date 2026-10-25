using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Helpers;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Infrastructure.Config;
using ClaimManagement.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ClaimManagement.Infrastructure.Repositories;

public class ClaimService : IClaimService
{
    ClaimDbContext db;
    IMapper mapper;
    ICurrentUser user;
    PolicyClient policyClient;
    PremiumClient premiumClient;
    int graceDays;

    // ASSUMED status values. Confirm the real strings with the Policy and Premium teammates.
    const string ActivePolicyStatus = "Active";
    const string PaidInstallmentStatus = "Paid";

    // recommended list based on the BRD product types (not an official BRD dropdown)
    static readonly string[] AllowedClaimTypes =
    {
        "Health", "Motor", "Life", "Travel", "Home", "Property", "Personal Accident"
    };

    public ClaimService(ClaimDbContext db, IMapper mapper, ICurrentUser user,
        PolicyClient policyClient, PremiumClient premiumClient, IConfiguration configuration)
    {
        this.db = db;
        this.mapper = mapper;
        this.user = user;
        this.policyClient = policyClient;
        this.premiumClient = premiumClient;

        // BRD: dues must be cleared "subject to product rules and grace period". The BRD gives no days.
        graceDays = int.TryParse(configuration["Eligibility:PremiumGraceDays"], out var d) ? d : 0;
    }

    public async Task<ClaimResponseDTO> AddClaimAsync(CreateClaimDTO dto, long performedBy)
    {
        // a customer can register claims only for themselves
        // INTERIM: skipped while the token has no customer_id claim
        if (user.IsCustomer && user.CustomerId.HasValue && dto.customer_id != user.CustomerId.Value)
            throw new BadRequestException("A customer can register claims only for themselves");

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

        // BRD: policy and premium eligibility (calls the Policy and Premium services)
        await CheckEligibilityAsync(dto.policy_id, dto.customer_id, dto.incident_date);

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

        // a customer sees only their own claims
        // INTERIM: skipped while the token has no customer_id claim
        if (user.IsCustomer && user.CustomerId.HasValue)
            filter.customer_id = user.CustomerId.Value;

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

        // a customer can open only their own claims
        AccessGuard.CheckClaimAccess(user, claim.customer_id);

        return claim;
    }

    // BRD (FR-CLM-001 and FR-CLM-002): policy active, not expired, premium up to date
    private async Task CheckEligibilityAsync(long policyId, long customerId, DateTime incidentDate)
    {
        // ---------- policy ----------
        PolicyInfoDTO? policy;
        try
        {
            policy = await policyClient.FindPolicyById(policyId);
        }
        catch (Exception ex) when (IsCallFailure(ex))
        {
            throw new ServiceUnavailableException("The Policy service is not available, so policy eligibility could not be verified. Please try again later.");
        }

        if (policy == null)
            throw new BadRequestException("Policy not found");

        // protects us from a wrong response shape (for example a wrapped response)
        if (policy.policy_id != policyId)
            throw new ServiceUnavailableException("The Policy service sent an unexpected response, so policy eligibility could not be verified.");

        if (policy.customer_id != customerId)
            throw new BadRequestException("This policy does not belong to the given customer");

        if (policy.policy_status != ActivePolicyStatus)
            throw new ConflictException("Claims can be registered only for an active policy. Current status: " + policy.policy_status);

        if (policy.expiry_date.Date < DateTime.Now.Date)
            throw new ConflictException("The policy has expired");

        if (incidentDate.Date < policy.effective_date.Date || incidentDate.Date > policy.expiry_date.Date)
            throw new BadRequestException("The incident date is outside the policy period");

        // TODO: coverage available, waiting period completed and claim limit (BRD FR-CLM-002) are NOT checked.
        // The policy response has no coverage, waiting period or limit fields.

        // ---------- premium ----------
        List<PremiumInstallmentDTO>? schedule;
        try
        {
            schedule = await premiumClient.FindScheduleByPolicy(policyId);
        }
        catch (Exception ex) when (IsCallFailure(ex))
        {
            throw new ServiceUnavailableException("The Premium service is not available, so premium status could not be verified. Please try again later.");
        }

        // decision: no schedule means premium status cannot be verified, so we do not pass it
        if (schedule == null || schedule.Count == 0)
            throw new ConflictException("No premium schedule was found for this policy, so premium status could not be verified");

        bool hasOverdue = schedule.Any(i =>
            i.status != PaidInstallmentStatus &&
            i.due_date.Date.AddDays(graceDays) < DateTime.Now.Date);

        if (hasOverdue)
            throw new ConflictException("Premium dues are pending. Claims cannot be registered until they are paid");
    }

    // timeout, connection problem, error status code, or a body that is not the expected JSON
    private static bool IsCallFailure(Exception ex)
    {
        return ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException;
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