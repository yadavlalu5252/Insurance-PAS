using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IAssessmentService
    {
        Task<AssessmentResponseDTO> SaveAssessmentAsync(long claimId, CreateAssessmentDTO dto, long performedBy);
        Task<AssessmentResponseDTO> FetchAssessmentAsync(long claimId);
    }
}
