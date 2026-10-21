using AutoMapper;
using InvestigationService.Application.DTOs;
using InvestigationService.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InvestigationService.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateInvestigationDto, Investigation>();

            CreateMap<SubmitInvestigationDto, Investigation>();
        }
    }
}