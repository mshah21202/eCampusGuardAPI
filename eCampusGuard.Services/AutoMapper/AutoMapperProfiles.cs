using System;
using AutoMapper;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Services.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));
        }
    }
}

