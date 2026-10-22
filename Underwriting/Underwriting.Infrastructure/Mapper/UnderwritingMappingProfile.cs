
using AutoMapper;
using Underwriting.Application.DTO;
using Underwriting.Domain.Entities;

namespace Underwriting.Infrastructure.Mapper
{
    public class UnderwritingMappingProfile : Profile
    {
        public UnderwritingMappingProfile()
        {
            CreateMap<savefinancialdto, financial_underwriting>();
            CreateMap<savemedicaldto, medical_underwriting>();
            CreateMap<createunderwritingdto, underwriting_case>();
        }
    }
}
