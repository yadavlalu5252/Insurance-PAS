using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface ISurveyService
    {
        Task<(string Result, SurveyResponseDTO? Survey)> AssignSurveyorAsync(long claimId, AssignSurveyDTO dto, long performedBy);
        Task<SurveyResponseDTO?> FetchSurveyByClaimAsync(long claimId);
        Task<string> SubmitReportAsync(long surveyId, DateTime inspectionDate, string extension, Stream content, long performedBy);
    }
}
