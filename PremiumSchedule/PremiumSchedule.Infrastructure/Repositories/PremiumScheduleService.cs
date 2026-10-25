using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;
using PremiumSchedule.Domain.Entities;
using PremiumSchedule.Infrastructure.Data;

namespace PremiumSchedule.Infrastructure.Repositories
{
    public class PremiumScheduleService : IPremiumSchedule
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IPolicyIssuanceClient policyIssuanceClient;
        private readonly IQuoteClient quoteClient;

        public PremiumScheduleService(
            AppDbContext db,
            IMapper mapper,
            IPolicyIssuanceClient policyIssuanceClient,
            IQuoteClient quoteClient)
        {
            this.db = db;
            this.mapper = mapper;
            this.policyIssuanceClient = policyIssuanceClient;
            this.quoteClient = quoteClient;
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

           

            var policy =
                await policyIssuanceClient.GetPolicyAsync(dto.policy_id);

            if (policy == null)
            {
                throw new InvalidOperationException(
                    "Policy not found.");
            }

           

            if (policy.policy_status != "Active")
            {
                throw new InvalidOperationException(
                    "Premium schedule can only be generated for an active policy.");
            }

            

            var quote =
                await quoteClient.GetQuoteByIdAsync(
                    policy.quote_id);

            if (quote == null)
            {
                throw new InvalidOperationException(
                    "Quote not found for the policy.");
            }

            
 

            if (!quote.premium_frequency_id.HasValue)
            {
                throw new InvalidOperationException(
                    "Premium frequency is not configured for this quote.");
            }

           

            string premiumFrequency =
                GetPremiumFrequencyName(
                    quote.premium_frequency_id.Value);

         
  

            int installmentCount =
                CalculateInstallmentCount(
                    policy.issue_date,
                    policy.expiry_date,
                    premiumFrequency);

        

            decimal installmentAmount =
                CalculateInstallmentAmount(
                    policy.premium_amount,
                    premiumFrequency);

      

            int months =
                GetFrequencyMonths(
                    premiumFrequency);

    

            for (int i = 1; i <= installmentCount; i++)
            {
                var schedule = new PremiumSchedules
                {
                    policy_id = policy.policy_id,

                    installment_no = i,

                    due_date = policy.effective_date.AddMonths(
                        (i - 1) * months),

                    premium_amount = installmentAmount,

                    late_fee = 0,

                    status = "Pending"
                };

                db.PremiumSchedule.Add(schedule);
            }

         

            await db.SaveChangesAsync();
        }


      

        private string GetPremiumFrequencyName(
            int premiumFrequencyId)
        {
            return premiumFrequencyId switch
            {
                1 => "Monthly",

                2 => "Quarterly",

                3 => "Half-Yearly",

                4 => "Yearly",

                _ => throw new ArgumentException(
                    $"Invalid premium frequency ID: {premiumFrequencyId}")
            };
        }




        public async Task<List<PremiumScheduleDto>>
            GetPremiumScheduleByPolicyId(int policy_id)
        {
            var schedules = await db.PremiumSchedule
                .Where(x => x.policy_id == policy_id)
                .OrderBy(x => x.installment_no)
                .ToListAsync();

            return mapper.Map<List<PremiumScheduleDto>>(
                schedules);
        }



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

            return mapper.Map<PremiumScheduleDto>(
                schedule);
        }


        private int CalculateInstallmentCount(
            DateTime issue_date,
            DateTime expiry_date,
            string frequency)
        {
            int months =
                GetFrequencyMonths(frequency);

            int totalMonths =
                ((expiry_date.Year - issue_date.Year) * 12)
                + expiry_date.Month
                - issue_date.Month;

            return (int)Math.Ceiling(
                (double)totalMonths / months);
        }




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


   

        private int GetGracePeriodDays(
            string frequency)
        {
            return frequency.ToLower() switch
            {
                "monthly" => 15,

                "quarterly" => 30,

                "half-yearly" => 30,

                "yearly" => 30,

                _ => throw new ArgumentException(
                    "Invalid premium frequency.")
            };
        }


   

        private decimal CalculateLateFee(
            DateTime dueDate,
            DateTime currentDate,
            string frequency,
            decimal dailyPenaltyRate,
            decimal maxLateFee)
        {
            int gracePeriodDays =
                GetGracePeriodDays(frequency);

            DateTime gracePeriodEndDate =
                dueDate.AddDays(gracePeriodDays);

            // Still inside grace period
            if (currentDate <= gracePeriodEndDate)
            {
                return 0;
            }

            int overdueDays =
                (currentDate - gracePeriodEndDate).Days;

            decimal lateFee =
                overdueDays * dailyPenaltyRate;

            // Maximum late fee
            if (lateFee > maxLateFee)
            {
                lateFee = maxLateFee;
            }

            return lateFee;
        }


       

        public async Task UpdateOverdueSchedules(
            decimal dailyPenaltyRate,
            decimal maxLateFee)
        {
            var schedules = await db.PremiumSchedule
                .Where(x =>
                    x.status == "Pending" ||
                    x.status == "Partial Paid")
                .ToListAsync();

            DateTime currentDate = DateTime.Now;

            foreach (var schedule in schedules)
            {
                

                var policy =
                    await policyIssuanceClient.GetPolicyAsync(
                        schedule.policy_id);

                if (policy == null)
                {
                    continue;
                }

              

                var quote =
                    await quoteClient.GetQuoteByIdAsync(
                        policy.quote_id);

                if (quote == null)
                {
                    continue;
                }


                if (!quote.premium_frequency_id.HasValue)
                {
                    continue;
                }

          

                string premiumFrequency;

                try
                {
                    premiumFrequency =
                        GetPremiumFrequencyName(
                            quote.premium_frequency_id.Value);
                }
                catch (ArgumentException)
                {
                    continue;
                }

             
      

                decimal lateFee =
                    CalculateLateFee(
                        schedule.due_date,
                        currentDate,
                        premiumFrequency,
                        dailyPenaltyRate,
                        maxLateFee);

                schedule.late_fee = lateFee;

             

                int gracePeriodDays =
                    GetGracePeriodDays(
                        premiumFrequency);

                DateTime gracePeriodEndDate =
                    schedule.due_date.AddDays(
                        gracePeriodDays);

                if (currentDate > gracePeriodEndDate)
                {
                    schedule.status = "Overdue";
                }
            }

            await db.SaveChangesAsync();
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

        
            if (schedule.status == "Paid")
            {
                throw new InvalidOperationException(
                    "Paid installments cannot be modified.");
            }

            
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





        public async Task<PremiumScheduleDto?>
            UpdateScheduleAfterPayment(
                int schedule_id,
                PaymentUpdateDto dto)
        {
            var schedule = await db.PremiumSchedule
                .FirstOrDefaultAsync(
                    x => x.schedule_id == schedule_id);

            if (schedule == null)
            {
                return null;
            }


            if (schedule.status == "Paid")
            {
                throw new InvalidOperationException(
                    "This premium schedule is already fully paid.");
            }

          
            if (dto.amount_paid >= schedule.premium_amount)
            {
                schedule.status = "Paid";
            }
   
            else if (dto.amount_paid > 0)
            {
                schedule.status = "Partial Paid";
            }

            await db.SaveChangesAsync();

            return mapper.Map<PremiumScheduleDto>(
                schedule);
        }
    }
}