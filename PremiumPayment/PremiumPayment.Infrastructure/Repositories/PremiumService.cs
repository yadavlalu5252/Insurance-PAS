using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Premium.Application.DTOs;
using Premium.Application.Interfaces;
using PremiumPayment.Infrastructure.Data;
using PremiumPaymentEntity = Premium.Domain.Entities.PremiumPayment;

namespace Premium.Application.Services
{
    public class PremiumService : IPremiumService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        private static readonly string[] AllowedPaymentModes =
        {
            "UPI",
            "Credit Card",
            "Debit Card",
            "Net Banking",
            "Cash",
            "Cheque",
            "DD",
            "Wallet"
        };

        private static readonly string[] OnlinePaymentModes =
        {
            "UPI",
            "Credit Card",
            "Debit Card",
            "Net Banking",
            "Wallet"
        };

        private static readonly string[] OfflinePaymentModes =
        {
            "Cash",
            "Cheque",
            "DD"
        };

        private static readonly string[] AllowedPaymentStatuses =
        {
            "Initiated",
            "Success",
            "Failed",
            "Pending",
            "Refunded",
            "Cancelled"
        };

        public PremiumService(
            AppDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<long> PayPremium(PremiumPaymentDto dto)
        {
            ValidatePaymentRequest(dto);

            var paymentMode = NormalizePaymentMode(dto.PaymentMode);

            var duplicateTransaction = await _context.PremiumPayments
                .AnyAsync(x =>
                    !string.IsNullOrWhiteSpace(dto.TransactionReference) &&
                    x.TransactionReference == dto.TransactionReference);

            if (duplicateTransaction)
            {
                throw new InvalidOperationException(
                    "Duplicate transaction reference is not allowed.");
            }

            var existingSuccessfulPayment = await _context.PremiumPayments
                .AnyAsync(x =>
                    x.ScheduleId == dto.ScheduleId &&
                    x.PolicyId == dto.PolicyId &&
                    x.PaymentStatus == "Success");

            if (existingSuccessfulPayment)
            {
                throw new InvalidOperationException(
                    "This premium installment has already been paid.");
            }

            var existingActivePayment = await _context.PremiumPayments
                .AnyAsync(x =>
                    x.ScheduleId == dto.ScheduleId &&
                    x.PolicyId == dto.PolicyId &&
                    (x.PaymentStatus == "Initiated" ||
                     x.PaymentStatus == "Pending"));

            if (existingActivePayment)
            {
                throw new InvalidOperationException(
                    "A payment for this premium installment is already in progress.");
            }

            var payment = _mapper.Map<PremiumPaymentEntity>(dto);

            payment.PaymentMode = paymentMode;
            payment.PaymentDate = DateTime.UtcNow;
            payment.PaymentStatus = "Initiated";

            _context.PremiumPayments.Add(payment);

            await _context.SaveChangesAsync();

            return payment.PaymentId;
        }

        public async Task<PremiumPaymentEntity> GetPayment(long paymentId)
        {
            var payment = await _context.PremiumPayments
                .FindAsync(paymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException(
                    "Payment not found.");
            }

            return payment;
        }

        public async Task<List<PremiumPaymentEntity>> GetPaymentHistory(
            long policyId)
        {
            if (policyId <= 0)
            {
                throw new ArgumentException(
                    "Policy ID must be greater than zero.");
            }

            return await _context.PremiumPayments
                .Where(x => x.PolicyId == policyId)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();
        }

        public async Task VerifyPayment(VerifyPaymentDto dto)
        {
            if (dto.PaymentId <= 0)
            {
                throw new ArgumentException(
                    "Payment ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentStatus))
            {
                throw new ArgumentException(
                    "Payment status is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.VerificationSource))
            {
                throw new ArgumentException(
                    "Verification source is required.");
            }

            var payment = await _context.PremiumPayments
                .FindAsync(dto.PaymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException(
                    "Payment not found.");
            }

            var newStatus = NormalizeStatus(dto.PaymentStatus);

            ValidateStatus(newStatus);

            var source = NormalizeVerificationSource(
                dto.VerificationSource);

            ValidateVerificationSource(payment, source);

            ValidateStatusTransition(
                payment.PaymentStatus,
                newStatus);

            if (!string.IsNullOrWhiteSpace(dto.TransactionReference))
            {
                var duplicateTransaction = await _context.PremiumPayments
                    .AnyAsync(x =>
                        x.PaymentId != payment.PaymentId &&
                        x.TransactionReference ==
                        dto.TransactionReference);

                if (duplicateTransaction)
                {
                    throw new InvalidOperationException(
                        "Transaction reference already belongs to another payment.");
                }

                payment.TransactionReference =
                    dto.TransactionReference;
            }

            payment.PaymentStatus = newStatus;

            await _context.SaveChangesAsync();

            if (newStatus == "Success")
            {
                await HandleSuccessfulPayment(payment);
            }
        }

        private static void ValidatePaymentRequest(
            PremiumPaymentDto dto)
        {
            if (dto.ScheduleId <= 0)
            {
                throw new ArgumentException(
                    "Schedule ID must be greater than zero.");
            }

            if (dto.PolicyId <= 0)
            {
                throw new ArgumentException(
                    "Policy ID must be greater than zero.");
            }

            if (dto.AmountPaid <= 0)
            {
                throw new ArgumentException(
                    "Amount paid must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMode))
            {
                throw new ArgumentException(
                    "Payment mode is required.");
            }

            var mode = NormalizePaymentMode(dto.PaymentMode);

            if (!AllowedPaymentModes.Contains(mode))
            {
                throw new ArgumentException(
                    "Invalid payment mode. Allowed modes are: UPI, Credit Card, Debit Card, Net Banking, Cash, Cheque, DD, Wallet.");
            }
        }

        private static string NormalizePaymentMode(
            string paymentMode)
        {
            var mode = paymentMode.Trim();

            if (mode.Equals(
                    "Demand Draft",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "DD";
            }

            var matchedMode = AllowedPaymentModes
                .FirstOrDefault(x =>
                    x.Equals(
                        mode,
                        StringComparison.OrdinalIgnoreCase));

            return matchedMode ?? mode;
        }

        private static string NormalizeStatus(
            string status)
        {
            var matchedStatus = AllowedPaymentStatuses
                .FirstOrDefault(x =>
                    x.Equals(
                        status.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            return matchedStatus ?? status.Trim();
        }

        private static void ValidateStatus(
            string status)
        {
            if (!AllowedPaymentStatuses.Contains(status))
            {
                throw new ArgumentException(
                    "Invalid payment status. Allowed statuses are: Initiated, Success, Failed, Pending, Refunded, Cancelled.");
            }
        }

        private static string NormalizeVerificationSource(
            string source)
        {
            if (source.Equals(
                    "Gateway",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Gateway";
            }

            if (source.Equals(
                    "Finance",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Finance";
            }

            throw new ArgumentException(
                "Verification source must be Gateway or Finance.");
        }

        private static void ValidateVerificationSource(
            PremiumPaymentEntity payment,
            string source)
        {
            if (OnlinePaymentModes.Contains(
                    payment.PaymentMode))
            {
                if (source != "Gateway")
                {
                    throw new InvalidOperationException(
                        "Online payments must be verified by the payment gateway.");
                }
            }

            if (OfflinePaymentModes.Contains(
                    payment.PaymentMode))
            {
                if (source != "Finance")
                {
                    throw new InvalidOperationException(
                        "Offline payments must be verified by the finance team.");
                }
            }
        }

        private static void ValidateStatusTransition(
            string currentStatus,
            string newStatus)
        {
            if (currentStatus == "Success")
            {
                if (newStatus != "Refunded")
                {
                    throw new InvalidOperationException(
                        "A successful payment cannot be changed to this status.");
                }

                return;
            }

            if (currentStatus == "Refunded")
            {
                throw new InvalidOperationException(
                    "A refunded payment cannot be modified.");
            }

            if (currentStatus == "Cancelled")
            {
                throw new InvalidOperationException(
                    "A cancelled payment cannot be modified.");
            }

            if (currentStatus == "Failed")
            {
                if (newStatus != "Pending" &&
                    newStatus != "Success")
                {
                    throw new InvalidOperationException(
                        "A failed payment can only be retried or verified successfully.");
                }

                return;
            }

            if (currentStatus == "Initiated")
            {
                if (newStatus != "Pending" &&
                    newStatus != "Success" &&
                    newStatus != "Failed" &&
                    newStatus != "Cancelled")
                {
                    throw new InvalidOperationException(
                        "Invalid status transition from Initiated.");
                }

                return;
            }

            if (currentStatus == "Pending")
            {
                if (newStatus != "Success" &&
                    newStatus != "Failed" &&
                    newStatus != "Cancelled")
                {
                    throw new InvalidOperationException(
                        "Invalid status transition from Pending.");
                }
            }
        }

        private async Task HandleSuccessfulPayment(
            PremiumPaymentEntity payment)
        {
            await _context.SaveChangesAsync();
        }
    }
}