using ClaimManagement.Application.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class LogOnlyNotificationService : INotificationService
    {
        ILogger<LogOnlyNotificationService> logger;

        public LogOnlyNotificationService(ILogger<LogOnlyNotificationService> logger)
        {
            this.logger = logger;
        }

        public Task NotifyAsync(long customerId, string eventName, string message)
        {
            logger.LogWarning(
                "NOTIFICATION NOT SENT (temporary stub). Customer {CustomerId}, event '{Event}': {Message}",
                customerId, eventName, message);

            return Task.CompletedTask;
        }
    }
}
