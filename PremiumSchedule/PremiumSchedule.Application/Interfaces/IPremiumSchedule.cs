using PremiumSchedule.Application.DTO;
using PremiumSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.Interfaces
{
    public interface IPremiumSchedule
    {
        Task GeneratePremiumSchedule(GeneratePremiumScheduleDto dto);

        Task<List<PremiumScheduleDto>> GetPremiumScheduleByPolicyId(int policy_id);

        Task<PremiumScheduleDto?> GetPremiumScheduleById(int schedule_id);

        Task<PremiumScheduleDto?>UpdatePremiumSchedule(int schedule_id,UpdatePremiumScheduleDto dto);

        Task UpdateOverdueSchedules(
   decimal dailyPenaltyRate,
   decimal maxLateFee);
    }
}
