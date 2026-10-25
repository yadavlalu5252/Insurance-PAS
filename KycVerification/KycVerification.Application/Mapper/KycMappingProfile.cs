using AutoMapper;
using KycVerification.Application.DTO;
using KycVerification.Domain.Entities;

namespace KycVerification.Application.Mapper
{
    public class KycMappingProfile : Profile
    {
        public KycMappingProfile()
        {
            // =========================
            // Customer KYC
            // =========================

            CreateMap<CreateCustomerKycDto, CustomerKyc>()
                .ForMember(
                    dest => dest.customer_id,
                    opt => opt.MapFrom(src => src.CustomerId)
                );

            CreateMap<CustomerKyc, CustomerKycDto>()
                .ForMember(
                    dest => dest.KycId,
                    opt => opt.MapFrom(src => src.kyc_id)
                )
                .ForMember(
                    dest => dest.CustomerId,
                    opt => opt.MapFrom(src => src.customer_id)
                )
                .ForMember(
                    dest => dest.KycNumber,
                    opt => opt.MapFrom(src => src.kyc_number)
                )
                .ForMember(
                    dest => dest.KycDate,
                    opt => opt.MapFrom(src => src.kyc_date)
                )
                .ForMember(
                    dest => dest.VerificationStatus,
                    opt => opt.MapFrom(src => src.verification_status)
                )
                .ForMember(
                    dest => dest.Remarks,
                    opt => opt.MapFrom(src => src.remarks)
                )
                .ForMember(
                    dest => dest.VerifiedBy,
                    opt => opt.MapFrom(src => src.verified_by)
                )
                .ForMember(
                    dest => dest.VerifiedDate,
                    opt => opt.MapFrom(src => src.verified_date)
                );

            // =========================
            // CKYC Details
            // =========================

            CreateMap<CkycDetails, CkycDetailsDto>()
                .ForMember(
                    dest => dest.CkycId,
                    opt => opt.MapFrom(src => src.CkycId)
                )
                .ForMember(
                    dest => dest.CustomerId,
                    opt => opt.MapFrom(src => src.CustomerId)
                )
                .ForMember(
                    dest => dest.CkycNumber,
                    opt => opt.MapFrom(src => src.CkycNumber)
                )
                .ForMember(
                    dest => dest.VerificationDate,
                    opt => opt.MapFrom(src => src.VerificationDate)
                )
                .ForMember(
                    dest => dest.CkycStatus,
                    opt => opt.MapFrom(src => src.CkycStatus)
                );

            CreateMap<CkycDetailsDto, CkycDetails>()
                .ForMember(
                    dest => dest.CkycId,
                    opt => opt.MapFrom(src => src.CkycId)
                )
                .ForMember(
                    dest => dest.CustomerId,
                    opt => opt.MapFrom(src => src.CustomerId)
                )
                .ForMember(
                    dest => dest.CkycNumber,
                    opt => opt.MapFrom(src => src.CkycNumber)
                )
                .ForMember(
                    dest => dest.VerificationDate,
                    opt => opt.MapFrom(src => src.VerificationDate)
                )
                .ForMember(
                    dest => dest.CkycStatus,
                    opt => opt.MapFrom(src => src.CkycStatus)
                );

            // =========================
            // KYC Document
            // =========================

            CreateMap<KycDocument, KycDocumentDto>()
                .ForMember(
                    dest => dest.DocumentId,
                    opt => opt.MapFrom(src => src.DocumentId)
                )
                .ForMember(
                    dest => dest.KycId,
                    opt => opt.MapFrom(src => src.KycId)
                )
                .ForMember(
                    dest => dest.DocumentType,
                    opt => opt.MapFrom(src => src.DocumentType)
                )
                .ForMember(
                    dest => dest.DocumentNumber,
                    opt => opt.MapFrom(src => src.DocumentNumber)
                )
                .ForMember(
                    dest => dest.FilePath,
                    opt => opt.MapFrom(src => src.FilePath)
                )
                .ForMember(
                    dest => dest.UploadDate,
                    opt => opt.MapFrom(src => src.UploadDate)
                )
                .ForMember(
                    dest => dest.VerificationStatus,
                    opt => opt.MapFrom(src => src.VerificationStatus)
                );

            CreateMap<UploadKycDocumentDto, KycDocument>()
                .ForMember(
                    dest => dest.KycId,
                    opt => opt.MapFrom(src => src.KycId)
                )
                .ForMember(
                    dest => dest.DocumentType,
                    opt => opt.MapFrom(src => src.DocumentType)
                )
                .ForMember(
                    dest => dest.DocumentNumber,
                    opt => opt.MapFrom(src => src.DocumentNumber)
                )
                .ForMember(
                    dest => dest.FilePath,
                    opt => opt.Ignore()
                )
                .ForMember(
                    dest => dest.DocumentId,
                    opt => opt.Ignore()
                )
                .ForMember(
                    dest => dest.UploadDate,
                    opt => opt.Ignore()
                )
                .ForMember(
                    dest => dest.VerificationStatus,
                    opt => opt.Ignore()
                );

            // =========================
            // Document Type Master
            // =========================

            CreateMap<DocumentTypeMaster, DocumentTypeMasterDto>()
                .ForMember(
                    dest => dest.DocumentTypeId,
                    opt => opt.MapFrom(src => src.DocumentTypeId)
                )
                .ForMember(
                    dest => dest.DocumentName,
                    opt => opt.MapFrom(src => src.DocumentName)
                )
                .ForMember(
                    dest => dest.MandatoryFlag,
                    opt => opt.MapFrom(src => src.MandatoryFlag)
                )
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status)
                );

            CreateMap<DocumentTypeMasterDto, DocumentTypeMaster>()
                .ForMember(
                    dest => dest.DocumentTypeId,
                    opt => opt.MapFrom(src => src.DocumentTypeId)
                )
                .ForMember(
                    dest => dest.DocumentName,
                    opt => opt.MapFrom(src => src.DocumentName)
                )
                .ForMember(
                    dest => dest.MandatoryFlag,
                    opt => opt.MapFrom(src => src.MandatoryFlag)
                )
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status)
                );

            // =========================
            // KYC Verification Log
            // =========================

            CreateMap<KycVerificationLog, KycVerificationLogDto>()
                .ForMember(
                    dest => dest.LogId,
                    opt => opt.MapFrom(src => src.log_id)
                )
                .ForMember(
                    dest => dest.KycId,
                    opt => opt.MapFrom(src => src.kyc_id)
                )
                .ForMember(
                    dest => dest.Action,
                    opt => opt.MapFrom(src => src.action)
                )
                .ForMember(
                    dest => dest.PerformedBy,
                    opt => opt.MapFrom(src => src.performed_by)
                )
                .ForMember(
                    dest => dest.ActionDate,
                    opt => opt.MapFrom(src => src.action_date)
                )
                .ForMember(
                    dest => dest.Remarks,
                    opt => opt.MapFrom(src => src.remarks)
                );

            CreateMap<KycVerificationLogDto, KycVerificationLog>()
                .ForMember(
                    dest => dest.log_id,
                    opt => opt.MapFrom(src => src.LogId)
                )
                .ForMember(
                    dest => dest.kyc_id,
                    opt => opt.MapFrom(src => src.KycId)
                )
                .ForMember(
                    dest => dest.action,
                    opt => opt.MapFrom(src => src.Action)
                )
                .ForMember(
                    dest => dest.performed_by,
                    opt => opt.MapFrom(src => src.PerformedBy)
                )
                .ForMember(
                    dest => dest.action_date,
                    opt => opt.MapFrom(src => src.ActionDate)
                )
                .ForMember(
                    dest => dest.remarks,
                    opt => opt.MapFrom(src => src.Remarks)
                );
        }
    }
}