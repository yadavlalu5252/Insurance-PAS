using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Domain.Rules
{
    public static class NotificationEvents
    {
        public const string ClaimRegistered = "Claim Registered";
        public const string DocumentsPending = "Documents Pending";
        public const string SurveyAssigned = "Survey Assigned";
        public const string ClaimApproved = "Claim Approved";
        public const string ClaimRejected = "Claim Rejected";
    }
}
