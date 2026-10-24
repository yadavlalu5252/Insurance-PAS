using System;
using System.Collections.Generic;
using System.Text;
using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using AgentManagement.Domain.Entities;
using AgentManagement.Infrastructure.Data;
using AutoMapper;
using global::AgentManagement.Application.DTO;
using global::AgentManagement.Application.Interface;
using global::AgentManagement.Domain.Entities;
using global::AgentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace AgentManagement.Infrastructure.Service
{

    namespace AgentManagement.Infrastructure.Service
    {
        public class AgentCommissionPaymentService : IAgentCommissionPaymentService
        {
            private readonly AppDbContext db;
            private readonly IMapper mapper;

            public AgentCommissionPaymentService(
                AppDbContext db,
                IMapper mapper)
            {
                this.db = db;
                this.mapper = mapper;
            }


            public async Task<AgentCommissionPaymentDTO2> CreatePayout(
                AgentCommissionPaymentDTO dto)
            {
                if (dto.AgentId <= 0)
                    throw new ArgumentException("Invalid AgentId");

                if (string.IsNullOrWhiteSpace(dto.PaymentMode))
                    throw new ArgumentException("PaymentMode is required");

                var validModes = new[] { "BankTransfer", "NEFT", "IMPS" };

                if (!validModes.Contains(dto.PaymentMode))
                    throw new ArgumentException("Invalid PaymentMode");

                var agentExists = await db.Agents
                    .AnyAsync(x => x.AgentId == dto.AgentId);

                if (!agentExists)
                    throw new KeyNotFoundException("Agent not found");

                // Allow only one active payout per agent at a time.
                var activePayoutExists = await db.AgentCommissionPayments
                    .AnyAsync(x => x.AgentId == dto.AgentId &&
                        (x.Status == "Pending" || x.Status == "Processing"));

                if (activePayoutExists)
                    throw new InvalidOperationException(
                        "This agent already has a pending payout");

                // Get commissions approved for this agent.
                var commissions = await db.AgentCommissions
                    .Where(x => x.AgentId == dto.AgentId &&
                                x.Status == "Approved")
                    .ToListAsync();

                if (commissions.Count == 0)
                    throw new InvalidOperationException(
                        "No approved commissions available for payout");

                var totalAmount = commissions.Sum(x => x.CommissionAmount);

                var payment = new AgentCommissionPayment
                {
                    AgentId = dto.AgentId,
                    PaymentReference = "PAY-" + Guid.NewGuid().ToString("N"),
                    TotalAmount = totalAmount,
                    PaymentMode = dto.PaymentMode,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                // Reserve these commissions so they cannot be paid twice.
                foreach (var commission in commissions)
                {
                    commission.Status = "Payable";
                }

                db.AgentCommissionPayments.Add(payment);

                await db.SaveChangesAsync();

                return mapper.Map<AgentCommissionPaymentDTO2>(payment);
            }


            public async Task<List<AgentCommissionPaymentDTO2>> FetchPayouts(
                string? paymentMode,
                string? status)
            {
                var query = db.AgentCommissionPayments.AsQueryable();

                if (!string.IsNullOrWhiteSpace(paymentMode))
                    query = query.Where(x => x.PaymentMode == paymentMode);

                if (!string.IsNullOrWhiteSpace(status))
                    query = query.Where(x => x.Status == status);

                var payments = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return mapper.Map<List<AgentCommissionPaymentDTO2>>(payments);
            }

            public async Task<List<AgentCommissionPaymentDTO2>> FetchPayoutsByAgent(
                long agentId)
            {
                var payments = await db.AgentCommissionPayments
                    .Where(x => x.AgentId == agentId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return mapper.Map<List<AgentCommissionPaymentDTO2>>(payments);
            }

            public async Task ReconcilePayment(
                long paymentId,
                string transactionReference,
                string status)
            {
                if (string.IsNullOrWhiteSpace(transactionReference))
                    throw new ArgumentException("Transaction reference is required");

                if (status != "Success" && status != "Failed")
                    throw new ArgumentException(
                        "Status must be Success or Failed");

                var payment = await db.AgentCommissionPayments
                    .FirstOrDefaultAsync(x =>
                        x.CommissionPaymentId == paymentId);

                if (payment == null)
                    throw new KeyNotFoundException("Payment not found");

                if (payment.Status != "Pending" &&
                    payment.Status != "Processing")
                {
                    throw new InvalidOperationException(
                        "Only pending or processing payments can be reconciled");
                }

                // Find the commissions reserved for this agent's active payout.
                var payableCommissions = await db.AgentCommissions
                    .Where(x => x.AgentId == payment.AgentId &&
                                x.Status == "Payable")
                    .ToListAsync();

                var payableTotal = payableCommissions.Sum(x => x.CommissionAmount);

                if (payableCommissions.Count == 0 ||
                    payableTotal != payment.TotalAmount)
                {
                    throw new InvalidOperationException(
                        "The payable commissions do not match this payout");
                }

                payment.TransactionReference = transactionReference;
                payment.Status = status;

                if (status == "Success")
                {
                    payment.PaymentDate = DateTime.UtcNow;

                    foreach (var commission in payableCommissions)
                    {
                        commission.Status = "Paid";
                    }
                }
                else
                {
                    // Allow a failed payout to be retried.
                    foreach (var commission in payableCommissions)
                    {
                        commission.Status = "Approved";
                    }
                }

                await db.SaveChangesAsync();
            }

        }
    }

}
