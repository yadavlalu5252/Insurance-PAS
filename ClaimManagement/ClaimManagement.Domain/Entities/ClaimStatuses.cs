using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class ClaimStatuses
    {
        public const string Registered = "Registered";
        public const string PendingVerification = "Pending Verification";
        public const string UnderSurvey = "Under Survey";
        public const string UnderInvestigation = "Under Investigation";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Settled = "Settled";
        public const string Cancelled = "Cancelled";
    }
}
