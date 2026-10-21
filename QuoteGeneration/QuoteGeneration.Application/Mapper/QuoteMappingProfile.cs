using AutoMapper;

using QuoteGeneration.Domain.Entities;
using QuoteGeneration.Application.DTO;

namespace QuoteGeneration.Application.QuoteGeneration.Mapper
{
    public class QuoteMappingProfile : Profile
    {
        public QuoteMappingProfile()
        {
            CreateMap<Quote, QuoteDTO>();
            CreateMap<QuoteDTO, Quote>().ForMember(dest => dest.quote_id, opt => opt.Ignore());

            CreateMap<PremiumCalculation, PremiumCalculationDTO>();
            CreateMap<PremiumCalculationDTO, PremiumCalculation>().ForMember(dest => dest.calculation_id, opt => opt.Ignore());

            CreateMap<RiskAssessment, RiskAssessmentDTO>();
            CreateMap<RiskAssessmentDTO, RiskAssessment>() .ForMember(dest => dest.risk_id, opt => opt.Ignore());

            CreateMap<QuoteRider, QuoteRiderDTO>();
            CreateMap<QuoteRiderDTO, QuoteRider>() .ForMember(dest => dest.rider_selection_id, opt => opt.Ignore());

            CreateMap<DiscountMaster, DiscountMasterDTO>();
            CreateMap<DiscountMasterDTO, DiscountMaster>().ForMember(dest => dest.discount_id, opt => opt.Ignore());

            CreateMap<QuoteHistory, QuoteHistoryDTO>();
            CreateMap<QuoteHistoryDTO, QuoteHistory>().ForMember(dest => dest.history_id, opt => opt.Ignore());
        }
    }
}