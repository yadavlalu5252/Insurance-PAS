using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumPayment.Application.Interface
{
    public interface IPremiumScheduleClient
    {
        Task UpdatePaymentStatusAsync(
            int scheduleId,
            decimal amountPaid,
            int paymentId);
    }
}
