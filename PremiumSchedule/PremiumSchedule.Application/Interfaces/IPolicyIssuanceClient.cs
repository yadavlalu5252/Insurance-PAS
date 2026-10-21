using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.Interfaces
{
    public interface IPolicyIssuanceClient
    {
        Task<string?> GetPremiumFrequency(int policyId);
    }
}
