using AutoMapper;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;
using PremiumSchedule.Domain.Entities;
using PremiumSchedule.Infrastructure.Data;
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

        // Generate premium schedule automatically after policy is issued
        public async Task GeneratePremiumSchedule(int policy_id)
        {
            // Check whether schedule already exists
            bool scheduleExists = await db.PremiumSchedule
                .AnyAsync(x => x.policy_id == policy_id);

            if (scheduleExists)
            {
                return;
            }


            // Get policy
            var policy = await db.Policies
                .FirstOrDefaultAsync(x => x.policy_id == policy_id);

            if (policy == null)
            {
                throw new KeyNotFoundException(
                    "Policy not found.");
            }


            // Schedule should be generated only
            // after policy is issued/active
            if (policy.policy_status != "Active")
            {
                throw new InvalidOperationException(
                    "Premium schedule can be generated only for an active policy.");
            }


            /*
             * Premium frequency and premium amount are coming
             * from Quote because Policy does not contain
             * premium_frequency or premium_amount.
             */

            var quote = await db.Quotes
                .FirstOrDefaultAsync(
                    x => x.quote_id == policy.quote_id);

            if (quote == null)
            {
                throw new KeyNotFoundException(
                    "Quote not found for this policy.");
            }


            string frequency = quote.premium_frequency;

            decimal premiumAmount = quote.final_premium;


            // Calculate number of installments
            int installmentCount = CalculateInstallmentCount(
                policy.issue_date,
                policy.expiry_date,
                frequency);


            // Calculate months between installments
            int months = GetFrequencyMonths(frequency);


            for (int i = 1; i <= installmentCount; i++)
            {
                var schedule = new PremiumSchedules
                {
                    policy_id = policy_id,

                    premium_frequency = frequency,

                    installment_number = i,

                    due_date = policy.effective_date.AddMonths(
                        (i - 1) * months),

                    premium_amount = premiumAmount,

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
                .OrderBy(x => x.installment_number)
                .ToListAsync();


            // Entity -> DTO using AutoMapper
            return mapper.Map<List<PremiumScheduleDto>>(schedules);
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
            return mapper.Map<PremiumScheduleDto>(schedule);
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
        private int GetFrequencyMonths(string frequency)
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
    }
}
