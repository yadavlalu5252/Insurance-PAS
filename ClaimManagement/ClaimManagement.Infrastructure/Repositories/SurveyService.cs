using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Helpers;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Domain.Rules;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClaimManagement.Infrastructure.Repositories;

public class SurveyService : ISurveyService
{
    ClaimDbContext db;
    IMapper mapper;
    IFileStorageService storage;

    public SurveyService(ClaimDbContext db, IMapper mapper, IFileStorageService storage)
    {
        this.db = db;
        this.mapper = mapper;
        this.storage = storage;
    }

    public async Task<SurveyResponseDTO> AssignSurveyorAsync(long claimId, AssignSurveyDTO dto, long performedBy)
    {
        if (dto.surveyor_id <= 0)
            throw new BadRequestException("Surveyor id is required");

        var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == claimId);
        if (claim == null)
            throw new NotFoundException("Claim not found");

        bool already = await db.survey_assignment.AnyAsync(s => s.claim_id == claimId);
        if (already)
            throw new ConflictException("A surveyor is already assigned to this claim");

        if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.UnderSurvey))
            throw new ConflictException("A surveyor can be assigned only while the claim is Pending Verification");

        // TODO: surveyor_id is an employee from another module. It is NOT validated yet.

        var survey = new SurveyAssignment
        {
            claim_id = claimId,
            surveyor_id = dto.surveyor_id,
            assigned_date = DateTime.Now,
            survey_status = "Assigned"
        };
        db.survey_assignment.Add(survey);

        claim.claim_status = ClaimStatuses.UnderSurvey;
        db.claim_history.Add(CreateHistory(claimId, "Surveyor Assigned", performedBy, "Surveyor id: " + dto.surveyor_id));

        await db.SaveChangesAsync();
        return mapper.Map<SurveyResponseDTO>(survey);
    }

    public async Task<SurveyResponseDTO> FetchSurveyAsync(long claimId)
    {
        var survey = await db.survey_assignment.FirstOrDefaultAsync(s => s.claim_id == claimId);
        if (survey == null)
            throw new NotFoundException("No survey found for this claim");

        return mapper.Map<SurveyResponseDTO>(survey);
    }

    public async Task SubmitReportAsync(long surveyId, DateTime inspectionDate,
        string? fileName, long fileLength, Stream? content, long performedBy)
    {
        if (inspectionDate == default)
            throw new BadRequestException("Inspection date is required");
        if (inspectionDate.Date > DateTime.Now.Date)
            throw new BadRequestException("Inspection date cannot be in the future");

        string extension = FileValidator.Check(fileName, fileLength, content);

        var survey = await db.survey_assignment.FirstOrDefaultAsync(s => s.survey_id == surveyId);
        if (survey == null)
            throw new NotFoundException("Survey not found");

        if (survey.survey_status == "Completed")
            throw new ConflictException("The survey report is already submitted");

        var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == survey.claim_id);
        if (claim == null || claim.claim_status != ClaimStatuses.UnderSurvey)
            throw new ConflictException("The claim is no longer under survey");

        survey.report_path = await storage.SaveFileAsync(content!, extension);
        survey.inspection_date = inspectionDate;
        survey.survey_status = "Completed";

        db.claim_history.Add(CreateHistory(survey.claim_id, "Survey Report Submitted", performedBy, null));
        await db.SaveChangesAsync();
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