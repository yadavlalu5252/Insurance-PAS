using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IClaimWorkflowService
    {
        // "Success", "NotFound", "InvalidStatus" or "DocumentsNotVerified"
        Task<string> ApproveClaimAsync(long id, ApproveClaimDTO dto, long performedBy);

        // "Success", "NotFound" or "InvalidStatus"
        Task<string> RejectClaimAsync(long id, RejectClaimDTO dto, long performedBy);

        // "Success", "NotFound" or "InvalidStatus"
        Task<string> CancelClaimAsync(long id, long performedBy);
    }
}
