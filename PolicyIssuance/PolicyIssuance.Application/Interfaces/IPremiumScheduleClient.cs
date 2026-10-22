using PolicyIssuance.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyIssuance.Application.Interfaces
{
   public interface IPremiumScheduleClient
    {
        Task<bool> GeneratePremiumSchedule(
            GeneratePremiumScheduleDto request);
    }
}
