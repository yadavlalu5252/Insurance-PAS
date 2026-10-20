using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Domain.Entities;

namespace ClaimManagement.Application.Mapper
{
    public class MappingData : Profile
    {
        public MappingData()
        {
            CreateMap<ClaimMaster, ClaimResponseDto>();
            CreateMap<ClaimMaster, ClaimStatusResponseDto>();
            CreateMap<ClaimCreateDto, ClaimMaster>();
            CreateMap<ClaimUpdateDto, ClaimMaster>();
            CreateMap<ClaimDocument, DocumentResponseDTO>();
            CreateMap<SurveyAssignment, SurveyResponseDTO>()
            .ForMember(d => d.report_uploaded, o => o.MapFrom(s => s.report_path != null));
        }
    }
}