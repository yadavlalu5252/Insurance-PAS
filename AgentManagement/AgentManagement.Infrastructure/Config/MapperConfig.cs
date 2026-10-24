
using AutoMapper;
using AgentManagement.Application.DTO;
using AgentManagement.Domain.Entities;

namespace AgentManagement.Infrastructure.Config
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Agent, AgentDTO>().ReverseMap();
            CreateMap<Agent, AgentDTO2>().ReverseMap();

            CreateMap<AgentCustomer, AgentCustomerDTO>().ReverseMap();
            CreateMap<AgentCustomer, AgentCustomerDTO2>().ReverseMap();

            CreateMap<AgentPolicy, AgentPolicyDTO>().ReverseMap();
            CreateMap<AgentPolicy, AgentPolicyDTO2>().ReverseMap();


            CreateMap<CommissionRule, CommissionRuleDTO>().ReverseMap();
            CreateMap<CommissionRule, CommissionRuleDTO2>().ReverseMap();


            CreateMap<AgentCommission, AgentCommissionDTO2>();

            CreateMap<AgentCommissionPayment, AgentCommissionPaymentDTO2>();

        }
    }
}
