using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Domain.Rules;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class SurveyService : ISurveyService
    {
        ClaimDbContext db;
        IMapper mapper;
        IFileStorageService storage;
        INotificationService notification;

        public SurveyService(ClaimDbContext db, IMapper mapper, IFileStorageService storage, INotificationService notification)
        {
            this.db = db;
            this.mapper = mapper;
            this.storage = storage;
            this.notification = notification;
        }

        public async Task<(string Result, SurveyResponseDTO? Survey)> AssignSurveyorAsync(long claimId, AssignSurveyDTO dto, long performedBy)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == claimId);
            if (claim == null) return ("ClaimNotFound", null);

            if (await db.survey_assignment.AnyAsync(s => s.claim_id == claimId))
                return ("AlreadyAssigned", null);

            if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.UnderSurvey))
                return ("InvalidStatus", null);

            // TODO: check that surveyor_id exists in the Employee table. NOT done (other module).

            var survey = new SurveyAssignment
            {
                claim_id = claimId,
                surveyor_id = dto.surveyor_id,
                assigned_date = DateTime.Now,
                survey_status = SurveyStatuses.Assigned
            };

            claim.claim_status = ClaimStatuses.UnderSurvey;

            db.survey_assignment.Add(survey);
            db.claim_history.Add(CreateHistory(claimId, "Surveyor Assigned: " + dto.surveyor_id, performedBy, null));
            await db.SaveChangesAsync();

            await notification.NotifyAsync(claim.customer_id, NotificationEvents.SurveyAssigned,
                "A surveyor has been assigned to your claim " + claim.claim_number + ".");

            return ("Success", mapper.Map<SurveyResponseDTO>(survey));
        }

        public async Task<SurveyResponseDTO?> FetchSurveyByClaimAsync(long claimId)
        {
            var survey = await db.survey_assignment.FirstOrDefaultAsync(s => s.claim_id == claimId);
            return survey == null ? null : mapper.Map<SurveyResponseDTO>(survey);
        }

        public async Task<string> SubmitReportAsync(long surveyId, DateTime inspectionDate, string extension, Stream content, long performedBy)
        {
            var survey = await db.survey_assignment
                .Include(s => s.claim)
                .FirstOrDefaultAsync(s => s.survey_id == surveyId);

            if (survey == null || survey.claim == null) return "NotFound";

            if (survey.survey_status == SurveyStatuses.Completed) return "AlreadyCompleted";

            // e.g. the claim was rejected while the survey was still open
            if (survey.claim.claim_status != ClaimStatuses.UnderSurvey) return "InvalidStatus";

            if (inspectionDate.Date < survey.assigned_date.Date) return "InvalidDate";

            survey.report_path = await storage.SaveFileAsync(content, extension);

            survey.inspection_date = inspectionDate;
            survey.survey_status = SurveyStatuses.Completed;

            db.claim_history.Add(CreateHistory(survey.claim_id, "Survey Report Submitted", performedBy, null));
            await db.SaveChangesAsync();
            return "Success";
        }

        private static ClaimHistory CreateHistory(long claimId, string action, long performedBy, string? remarks)
        {
            return new ClaimHistory
            {
                claim_id = claimId,
                action = action,
                performed_by = performedBy,
                action_date = DateTime.Now,
                remarks = remarks
            };
        }
    }
}
