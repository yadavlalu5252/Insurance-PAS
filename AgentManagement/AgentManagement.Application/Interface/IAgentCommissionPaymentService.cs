using AgentManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Application.Interface
{
    public interface IAgentCommissionPaymentService
    {
        Task<AgentCommissionPaymentDTO2> CreatePayout(AgentCommissionPaymentDTO dto);

        Task<List<AgentCommissionPaymentDTO2>> FetchPayouts(
            string? paymentMode,
            string? status);

        Task<List<AgentCommissionPaymentDTO2>> FetchPayoutsByAgent(long agentId);

        Task ReconcilePayment(
            long paymentId,
            string transactionReference,
            string status);
    }
}
