using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IClaimWorkflowService
    {
        Task ApproveClaimAsync(long id, ApproveClaimDTO dto, long performedBy);
        Task RejectClaimAsync(long id, RejectClaimDTO dto, long performedBy);
        Task CancelClaimAsync(long id, long performedBy);
    }
}
