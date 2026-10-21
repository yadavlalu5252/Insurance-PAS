
using AutoMapper;
using CustomerRegistration.Application.DTO.Contact;
using CustomerRegistration.Application.DTOs.Address;
using CustomerRegistration.Application.DTOs.Contact;
using CustomerRegistration.Application.DTOs.Customer;
using CustomerRegistration.Application.DTOs.Document;
using CustomerRegistration.Application.DTOs.Nominee;
using CustomerRegistration.Domain.Entities;

namespace CustomerRegistration.Application.Mapper;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();
        CreateMap<Customer, CustomerResponseDto>();

        CreateMap<CreateCustomerAddressDto, CustomerAddress>();
        CreateMap<UpdateCustomerAddressDto, CustomerAddress>();
        CreateMap<CustomerAddress, CustomerAddressResponseDto>();

        CreateMap<CreateNomineeDto, Nominee>();
        CreateMap<UpdateNomineeDto, Nominee>();
        CreateMap<Nominee, NomineeResponseDto>();

        CreateMap<CustomerDocument, CustomerDocumentDto>();

        CreateMap<CustomerContactDto, CustomerContact>();
        CreateMap<CustomerContact, CustomerContactResponseDto>();
    }
}
