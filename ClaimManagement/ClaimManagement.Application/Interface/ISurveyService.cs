using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface ISurveyService
    {
        Task<SurveyResponseDTO> AssignSurveyorAsync(long claimId, AssignSurveyDTO dto, long performedBy);
        Task<SurveyResponseDTO> FetchSurveyAsync(long claimId);

        Task SubmitReportAsync(long surveyId, DateTime inspectionDate,
            string? fileName, long fileLength, Stream? content, long performedBy);
    }
}
