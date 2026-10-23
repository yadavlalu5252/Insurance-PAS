using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Helpers
{
    public static class AccessGuard
    {
        public static void CheckClaimAccess(ICurrentUser user, long claimCustomerId)
        {
            if (user.IsCustomer && user.CustomerId.HasValue && user.CustomerId.Value != claimCustomerId)
                throw new NotFoundException("Claim not found");
        }
    }
}
