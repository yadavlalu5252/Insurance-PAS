using Premium.Application.DTOs;
using PremiumPaymentEntity = Premium.Domain.Entities.PremiumPayment;

namespace Premium.Application.Interfaces
{
    public interface IPremiumService
    {
        Task<long> PayPremium(PremiumPaymentDto dto);

        Task<PremiumPaymentEntity> GetPayment(long paymentId);

        Task<List<PremiumPaymentEntity>> GetPaymentHistory(long policyId);

        Task VerifyPayment(VerifyPaymentDto dto);
    }
}