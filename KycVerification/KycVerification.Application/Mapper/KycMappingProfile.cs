using AutoMapper;
using KycVerification.Application.DTO;
using KycVerification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.Mapper
{
    public class KycMappingProfile : Profile
    {
        public KycMappingProfile()
        {
            // Customer KYC
            CreateMap<CreateCustomerKycDto, CustomerKyc>();
            CreateMap<CustomerKyc, CustomerKycDto>();
            CreateMap<CustomerKycDto, CustomerKyc>();

            // KYC Document
            CreateMap<UploadKycDocumentDto, KycDocument>();
            CreateMap<KycDocument, KycDocumentDto>();

            // CKYC Details
            CreateMap<CkycDetails, CkycDetailsDto>();
            CreateMap<CkycDetailsDto, CkycDetails>();

            // KYC Verification Log
            CreateMap<KycVerificationLog, KycVerificationLogDto>();
            CreateMap<KycVerificationLogDto, KycVerificationLog>();

            // Document Type Master
            CreateMap<DocumentTypeMaster, DocumentTypeMasterDto>();
            CreateMap<DocumentTypeMasterDto, DocumentTypeMaster>();
        }
    }
}
