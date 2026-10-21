
using AutoMapper;
using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Domain.Entities;
using PolicyIssuance.Domain.Enums;

namespace PolicyIssuance.Application.Config
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<PolicyEntity, PolicyDTO>()
                .ForMember(
                    dest => dest.policy_status,
                    opt => opt.MapFrom(
                        src => src.policy_status.ToString()))
                .ReverseMap()
                .ForMember(
                    dest => dest.policy_status,
                    opt => opt.MapFrom(
                        src => Enum.Parse<PolicyStatus>(
                            src.policy_status, true)));

            CreateMap<PolicyEntity, PolicyResponseDTO>()
                .ForMember(
                    dest => dest.policy_status,
                    opt => opt.MapFrom(
                        src => src.policy_status.ToString()))
                .ForMember(
                    dest => dest.customer_name,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.product_name,
                    opt => opt.Ignore());

            CreateMap<PolicyCoverage, PolicyCoverageDTO>()
                .ReverseMap();

            CreateMap<PolicyRider, PolicyRiderDTO>()
                .ReverseMap();

            CreateMap<PolicyDocument, PolicyDocumentDTO>()
                .ReverseMap();

            CreateMap<PolicyNominee, PolicyNomineeDTO>()
                .ReverseMap();

            CreateMap<PolicyEndorsement, PolicyEndorsementDTO>()
                .ReverseMap();

            CreateMap<PolicyHistory, PolicyHistoryDTO>()
                .ReverseMap();
            CreateMap<PolicyClosure, PolicyClosureDTO>().ReverseMap();

            CreateMap<PolicyClosure, PolicyClosureDTO2>().ReverseMap();
        }
    }
}
