using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface ICurrentUser
    {
        long UserId { get; }
        string Role { get; }
        long? CustomerId { get; }
        bool IsCustomer { get; }
    }
}
