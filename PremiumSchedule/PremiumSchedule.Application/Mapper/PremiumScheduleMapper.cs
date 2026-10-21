using AutoMapper;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.Mapper
{
    public class PremiumScheduleMapper:Profile
    {
        public PremiumScheduleMapper()
        {
            CreateMap<PremiumSchedules, PremiumScheduleDto>();
        }
    }
}
