using ClaimManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Domain.Rules
{
    public class ClaimStatusRules
    {
        static readonly Dictionary<string, string[]> Allowed = new()
        {
            { ClaimStatuses.Registered, new[] { ClaimStatuses.PendingVerification, ClaimStatuses.Cancelled } },

            { ClaimStatuses.PendingVerification, new[] {
                ClaimStatuses.UnderSurvey, ClaimStatuses.UnderInvestigation,
                ClaimStatuses.Approved, ClaimStatuses.Rejected, ClaimStatuses.Cancelled } },

            { ClaimStatuses.UnderSurvey, new[] {
                ClaimStatuses.UnderInvestigation, ClaimStatuses.Approved, ClaimStatuses.Rejected } },

            { ClaimStatuses.UnderInvestigation, new[] {
                ClaimStatuses.Approved, ClaimStatuses.Rejected } },

            // Settled is set by the Settlement service (Abhijit), not by this API
            { ClaimStatuses.Approved, new[] { ClaimStatuses.Settled } }
        };

        public static bool CanMove(string from, string to)
        {
            return Allowed.TryGetValue(from, out var list) && list.Contains(to);
        }
    }
}
