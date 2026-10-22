using AutoMapper;
using Premium.Application.DTOs;
using PremiumPayment.Application.DTO;
using PremiumPayment.Domain.Entities;

namespace Premium.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreatePaymentDto -> PremiumPayments
            CreateMap<CreatePaymentDto, PremiumPayments>();

            // PremiumPayments -> PaymentResponseDto
            CreateMap<PremiumPayments, PaymentResponseDto>();

            CreateMap<PaymentReceipt, ReceiptResponseDto>();
        }
    }
    
}