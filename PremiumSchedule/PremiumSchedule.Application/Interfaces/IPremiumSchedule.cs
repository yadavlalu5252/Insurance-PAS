using PremiumSchedule.Application.DTO;
using PremiumSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.Interfaces
{
    public interface IPremiumSchedule
    {
        Task GeneratePremiumSchedule(int policy_id);

        Task<List<PremiumScheduleDto>> GetPremiumScheduleByPolicyId(int policy_id);

        Task<PremiumScheduleDto?> GetPremiumScheduleById(int schedule_id);
    }
}
