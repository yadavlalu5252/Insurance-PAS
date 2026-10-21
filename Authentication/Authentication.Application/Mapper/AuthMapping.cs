using Authentication.Application.DTO;
using Authentication.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication.Application.Mapper
{
    public class AuthMapping : Profile
    {
        public AuthMapping()
        {
            CreateMap<User, LoginResponseDto>();
            CreateMap<RegisterUserDto, User>();
        }
    }
}
