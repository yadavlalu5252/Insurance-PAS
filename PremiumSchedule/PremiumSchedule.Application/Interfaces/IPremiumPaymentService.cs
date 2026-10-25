using Premium.Application.DTOs;
using PremiumSchedule.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.Interface
{
    public interface IPremiumPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(
CreatePaymentDto dto);

        Task<PaymentResponseDto?> GetPaymentByIdAsync(
            int paymentId);

        Task<IEnumerable<PaymentResponseDto>> GetPaymentsAsync();

       
        Task<PaymentResponseDto?> VerifyPaymentAsync(
            int paymentId,
            VerifyPaymentDto dto);

        Task<PaymentResponseDto?> RetryPaymentAsync(
            int paymentId);

        Task<PaymentResponseDto?> UpdatePaymentStatusAsync(
            int paymentId,
            PaymentStatusDto dto);

        Task<IEnumerable<PaymentResponseDto>>
            GetPaymentHistoryByPolicyAsync(int policyId);

        Task<ReceiptResponseDto?> GetReceiptAsync(
      int paymentId);

        Task<ReceiptResponseDto?> RegenerateReceiptAsync(
            int paymentId);

        Task<(byte[] FileBytes, string FileName)?>
    DownloadReceiptAsync(int paymentId);

        //Task<RefundResponseDto?> CreateRefundAsync(
        //    CreateRefundDto dto);

        //Task<RefundResponseDto?> ApproveRefundAsync(
        //    int refundId);

        //Task<RefundResponseDto?> GetRefundAsync(
        //    int refundId);
    }
}
