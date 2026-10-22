using AutoMapper;
using PolicyServicing.Application.DTO;
using PolicyServicing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.Mapper
{
    public class RenewalProfile : Profile
    {
        public RenewalProfile()
        {
            CreateMap<policy_renewal, RenewalDetailsResponseDTO>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Renewal_Status.ToString()));
            CreateMap<PolicyCancellation, CancellationDetailsDTO>();
        }
    }
}
