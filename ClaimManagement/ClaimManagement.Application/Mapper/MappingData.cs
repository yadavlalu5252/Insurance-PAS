using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Domain.Entities;

namespace ClaimManagement.Application.Mapper
{
    public class MappingData : Profile
    {
        public MappingData()
        {
            CreateMap<ClaimMaster, ClaimResponseDTO>();
            CreateMap<ClaimMaster, ClaimStatusDTO>();
            CreateMap<CreateClaimDTO, ClaimMaster>();
            CreateMap<UpdateClaimDTO, ClaimMaster>();
            CreateMap<ClaimHistory, ClaimHistoryDTO>();
            CreateMap<ClaimDocument, DocumentResponseDTO>();
            CreateMap<SurveyAssignment, SurveyResponseDTO>();
            CreateMap<ClaimAssessment, AssessmentResponseDTO>();
            CreateMap<InvestigationMaster, InvestigationResponseDTO>();
            CreateMap<InvestigationEvidence, EvidenceResponseDTO>();
        }
    }
}