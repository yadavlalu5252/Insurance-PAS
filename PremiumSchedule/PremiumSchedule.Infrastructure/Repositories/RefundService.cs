using AutoMapper;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Domain.Entities;
using PremiumSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using PremiumSchedule.Application.Interfaces;

namespace PremiumSchedule.Infrastructure.Repositories
{
    public class RefundService : IRefundService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public RefundService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        
        public async Task<RefundResponseDto> CreateRefundAsync(
            CreateRefundDto dto)
        {
           
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == dto.payment_id);

            if (payment == null)
            {
                throw new InvalidOperationException(
                    "Payment not found.");
            }

            if (payment.payment_status != PaymentStatus.Success)
            {
                throw new InvalidOperationException(
                    "Refund can only be created for a successful payment.");
            }

            
            decimal alreadyRefunded = await db.PremiumRefunds
                .Where(x =>
                    x.payment_id == dto.payment_id &&
                    (x.refund_status == RefundStatus.Approved ||
                     x.refund_status == RefundStatus.Completed))
                .SumAsync(x => x.refund_amount);

         
            if (alreadyRefunded + dto.refund_amount >
                payment.amount_paid)
            {
                throw new InvalidOperationException(
                    "Refund amount cannot exceed the payment amount.");
            }

          
            bool pendingRefundExists =
                await db.PremiumRefunds
                    .AnyAsync(x =>
                        x.payment_id == dto.payment_id &&
                        x.refund_status == RefundStatus.Pending);

            if (pendingRefundExists)
            {
                throw new InvalidOperationException(
                    "A refund request is already pending for this payment.");
            }

         
            var refund = new PremiumRefund
            {
                payment_id = dto.payment_id,
                refund_amount = dto.refund_amount,
                refund_date = DateTime.UtcNow,
                refund_status = RefundStatus.Pending,

              
                approved_by = 0
            };

            db.PremiumRefunds.Add(refund);

            await db.SaveChangesAsync();

            return mapper.Map<RefundResponseDto>(refund);
        }


  

        public async Task<RefundResponseDto?> ApproveRefundAsync(
            int refundId,
            ApproveRefundDto dto)
        {
           
            var refund = await db.PremiumRefunds
                .FirstOrDefaultAsync(x =>
                    x.refund_id == refundId);

            if (refund == null)
            {
                return null;
            }

          
            if (refund.refund_status != RefundStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending refunds can be approved.");
            }

           
            var payment = await db.PremiumPayments
                .FirstOrDefaultAsync(x =>
                    x.payment_id == refund.payment_id);

            if (payment == null)
            {
                throw new InvalidOperationException(
                    "Original payment not found.");
            }

       
            if (payment.payment_status != PaymentStatus.Success)
            {
                throw new InvalidOperationException(
                    "Refund cannot be approved because the original payment is not successful.");
            }

           
            decimal alreadyRefunded =
                await db.PremiumRefunds
                    .Where(x =>
                        x.payment_id == refund.payment_id &&
                        (x.refund_status == RefundStatus.Approved ||
                         x.refund_status == RefundStatus.Completed) &&
                        x.refund_id != refund.refund_id)
                    .SumAsync(x => x.refund_amount);

           
            if (alreadyRefunded + refund.refund_amount >
                payment.amount_paid)
            {
                throw new InvalidOperationException(
                    "Total refund amount cannot exceed the payment amount.");
            }

           
            refund.refund_status =
                RefundStatus.Approved;

            refund.approved_by =
                dto.approved_by;

            refund.refund_date =
                DateTime.UtcNow;

            await db.SaveChangesAsync();

            return mapper.Map<RefundResponseDto>(
                refund);
        }


        

        public async Task<RefundResponseDto?> GetRefundByIdAsync(
            int refundId)
        {
            var refund = await db.PremiumRefunds
                .FirstOrDefaultAsync(x =>
                    x.refund_id == refundId);

            if (refund == null)
            {
                return null;
            }

            return mapper.Map<RefundResponseDto>(
                refund);
        }
    }
}
