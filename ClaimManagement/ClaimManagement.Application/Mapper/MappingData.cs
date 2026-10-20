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
        }
    }
}