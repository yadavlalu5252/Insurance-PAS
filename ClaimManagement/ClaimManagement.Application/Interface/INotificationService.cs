using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface INotificationService
    {
        Task NotifyAsync(long customerId, string eventName, string message);
    }
}
