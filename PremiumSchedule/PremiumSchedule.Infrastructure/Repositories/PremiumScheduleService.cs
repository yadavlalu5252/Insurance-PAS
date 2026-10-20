using AutoMapper;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;
using PremiumSchedule.Domain.Entities;
using PremiumSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Infrastructure.Repositories
{
    public class PremiumScheduleService:IPremiumSchedule
    {
        AppDbContext db;
        IMapper mapper;
        public PremiumScheduleService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task GeneratePremiumSchedule(
            GeneratePremiumScheduleDto dto)
        {
            bool scheduleExists = await db.PremiumSchedule
                .AnyAsync(x => x.policy_id == dto.policy_id);

            if (scheduleExists)
            {
                return;
            }

            int installmentCount = CalculateInstallmentCount(
                dto.issue_date,
                dto.expiry_date,
                dto.premium_frequency);

            decimal installmentAmount =
                CalculateInstallmentAmount(
                    dto.total_premium,
                    dto.premium_frequency);

            int months = GetFrequencyMonths(
                dto.premium_frequency);

            for (int i = 1; i <= installmentCount; i++)
            {
                var schedule = new PremiumSchedules
                {
                    policy_id = dto.policy_id,

                    installment_no = i,

                    due_date = dto.effective_date.AddMonths(
                        (i - 1) * months),

                    premium_amount = installmentAmount,

                    late_fee = 0,

                    status = "Pending"
                };

                db.PremiumSchedule.Add(schedule);
            }

            await db.SaveChangesAsync();
        }


        // Get all installments for one policy
        public async Task<List<PremiumScheduleDto>>
            GetPremiumScheduleByPolicyId(int policy_id)
        {
            var schedules = await db.PremiumSchedule
                .Where(x => x.policy_id == policy_id)
                .OrderBy(x => x.installment_no)
                .ToListAsync();

            // Entity -> DTO using AutoMapper
            return mapper.Map<List<PremiumScheduleDto>>(
                schedules);
        }


        // Get one schedule record
        public async Task<PremiumScheduleDto?>
            GetPremiumScheduleById(int schedule_id)
        {
            var schedule = await db.PremiumSchedule
                .FirstOrDefaultAsync(
                    x => x.schedule_id == schedule_id);

            if (schedule == null)
            {
                return null;
            }

            // Entity -> DTO using AutoMapper
            return mapper.Map<PremiumScheduleDto>(
                schedule);
        }


        // Calculate number of installments
        private int CalculateInstallmentCount(
            DateTime issue_date,
            DateTime expiry_date,
            string frequency)
        {
            int months = GetFrequencyMonths(frequency);

            int totalMonths =
                ((expiry_date.Year - issue_date.Year) * 12)
                + expiry_date.Month
                - issue_date.Month;

            return (int)Math.Ceiling(
                (double)totalMonths / months);
        }


        // Convert frequency into months
        private int GetFrequencyMonths(
            string frequency)
        {
            return frequency.ToLower() switch
            {
                "monthly" => 1,

                "quarterly" => 3,

                "half-yearly" => 6,

                "yearly" => 12,

                _ => throw new ArgumentException(
                    "Invalid premium frequency.")
            };
        }


        // Calculate installment amount
        // based on total premium and frequency
        private decimal CalculateInstallmentAmount(
            decimal totalPremium,
            string frequency)
        {
            return frequency.ToLower() switch
            {
                "monthly" => totalPremium / 12,

                "quarterly" => totalPremium / 4,

                "half-yearly" => totalPremium / 2,

                "yearly" => totalPremium,

                _ => throw new ArgumentException(
                    "Invalid premium frequency.")
            };
        }

        public async Task<PremiumScheduleDto?>
    UpdatePremiumSchedule(
        int schedule_id,
        UpdatePremiumScheduleDto dto)
        {
            var schedule = await db.PremiumSchedule
                .FirstOrDefaultAsync(
                    x => x.schedule_id == schedule_id);

            if (schedule == null)
            {
                return null;
            }

            // Paid installments cannot be modified
            if (schedule.status == "Paid")
            {
                throw new InvalidOperationException(
                    "Paid installments cannot be modified.");
            }

            // Validate status
            var allowedStatuses = new[]
            {
        "Pending",
        "Paid",
        "Partial Paid",
        "Overdue",
        "Cancelled"
    };

            if (!allowedStatuses.Contains(dto.status))
            {
                throw new ArgumentException(
                    "Invalid installment status.");
            }

            schedule.late_fee = dto.late_fee;
            schedule.status = dto.status;

            await db.SaveChangesAsync();

            return mapper.Map<PremiumScheduleDto>(
                schedule);
        }
    }
}
