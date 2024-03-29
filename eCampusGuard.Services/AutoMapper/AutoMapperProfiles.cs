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
            CreateMap<VehicleDto, Vehicle>();

            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));

            CreateMap<PermitApplication, PermitApplicationInfoDto>()
                .ForMember(dest => dest.PermitName, opt => opt.MapFrom(src => src.Permit.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.AcademicYear));

            CreateMap<PermitApplication, PermitApplicationDto>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.Permit, opt => opt.MapFrom(src => src.Permit));

            CreateMap<PermitApplicationDto, PermitApplication>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.Permit, opt => opt.MapFrom(src => src.Permit));

            CreateMap<CreatePermitApplicationDto, PermitApplication>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.PermitId, opt => opt.MapFrom(src => src.PermitId));

            CreateMap<Permit, PermitDto>()
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area));
            CreateMap<PermitDto, Permit>();

            CreateMap<Area, AreaDto>();
            CreateMap<AreaDto, Area>();

        }
    }
}

