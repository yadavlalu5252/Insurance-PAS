using PremiumSchedule.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.Interfaces
{
    public interface IRefundService
    {
        Task<RefundResponseDto> CreateRefundAsync(
            CreateRefundDto dto);

        Task<RefundResponseDto?> ApproveRefundAsync(
            int refundId,
            ApproveRefundDto dto);

        Task<RefundResponseDto?> GetRefundByIdAsync(
            int refundId);
    }
}
