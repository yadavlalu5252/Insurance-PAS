using AutoMapper;
using Premium.Application.DTOs;
using Premium.Domain.Entities;

namespace Premium.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PremiumPaymentDto, PremiumPayment>();

            CreateMap<PremiumPayment, PremiumPaymentDto>();
        }
    }
}