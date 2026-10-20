
using AutoMapper;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Domain.Entities;

namespace InsuranceProductManagement.Application.Mapper
{
    public class ProductCategoryMappingProfile : Profile
    {
        public ProductCategoryMappingProfile()
        {
        
            CreateMap<ProductCategory, ProductCategoryDTO>();

            CreateMap<ProductCategoryDTO, ProductCategory>().ForMember(dest => dest.category_id,opt => opt.Ignore());


          
            CreateMap<InsuranceProduct, InsuranceProductDTO>();

            CreateMap<InsuranceProductDTO, InsuranceProduct>().ForMember(dest => dest.product_id,opt => opt.Ignore());


    
            CreateMap<CoverageMaster, CoverageMasterDTO>();

            CreateMap<CoverageMasterDTO, CoverageMaster>().ForMember(dest => dest.coverage_id,opt => opt.Ignore());


        
            CreateMap<RiderMaster, RiderMasterDTO>();

            CreateMap<RiderMasterDTO, RiderMaster>() .ForMember(dest => dest.rider_id,opt => opt.Ignore());

            CreateMap<BenefitMaster, BenefitMasterDTO>();

            CreateMap<BenefitMasterDTO, BenefitMaster>().ForMember(dest => dest.benefit_id,opt => opt.Ignore());


    
            CreateMap<PremiumFrequency, PremiumFrequencyDTO>();

            CreateMap<PremiumFrequencyDTO, PremiumFrequency>().ForMember(dest => dest.frequency_id, opt => opt.Ignore());


         
            CreateMap<ProductCommission, ProductCommissionDTO>();

            CreateMap<ProductCommissionDTO, ProductCommission>().ForMember(dest => dest.commission_id,opt => opt.Ignore());
        }
    }
}
