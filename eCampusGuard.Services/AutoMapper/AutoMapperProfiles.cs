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
            CreateMap<Vehicle, VehicleDto>();

            CreateMap<UserNotification, NotificationDto>()
                .ForMember(dest => dest.Read, opt => opt.MapFrom(src => src.Read))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Notification.Title))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Notification.Body))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Notification.Timestamp))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Notification.Id));

            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.UserName));

            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));

            CreateMap<PermitApplication, PermitApplicationInfoDto>()
                .ForMember(dest => dest.PermitName, opt => opt.MapFrom(src => src.Permit.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => int.Parse(src.User.UserName)))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.Name));

            CreateMap<PermitApplication, PermitApplicationDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => int.Parse(src.User.UserName)))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.Permit, opt => opt.MapFrom(src => src.Permit))
                .ForMember(dest => dest.UserPermitId, opt => opt.MapFrom(src => src.UserPermitId));

            CreateMap<PermitApplicationDto, PermitApplication>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.Permit, opt => opt.MapFrom(src => src.Permit));

            CreateMap<CreatePermitApplicationDto, PermitApplication>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.PermitId, opt => opt.MapFrom(src => src.PermitId))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.AcademicYear));

            CreateMap<Area, AreaDto>()
                .ForMember(dest => dest.Occupied, opt => opt.MapFrom(src => src.Occupied));
            CreateMap<AreaDto, Area>()
                .ForMember(dest => dest.Occupied, opt => opt.MapFrom(src => src.Occupied))
                .ForMember(dest => dest.CurrentOccupied, opt => opt.MapFrom(src => 0));

            CreateMap<Area, AreaScreenDto>()
                .ForMember(dest => dest.Occupied, opt => opt.MapFrom(src => src.CurrentOccupied))
                .ForMember(dest => dest.AccessLogs, opt => opt.MapFrom(src => src.AccessLogs));



            CreateMap<Permit, PermitDto>()
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area));
            CreateMap<PermitDto, Permit>();


            CreateMap<UserPermit, UserPermitDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.Permit, opt => opt.MapFrom(src => src.Permit))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsPermitValid() ? src.Status : UserPermitStatus.Expired))
                .ForMember(dest => dest.Expiry, opt => opt.MapFrom(src => src.Permit.Expiry))
                .ForMember(dest => dest.PermitApplication, opt => opt.MapFrom(src => src.PermitApplication));

            CreateMap<UpdateRequest, UpdateRequestDto>()
                .ForMember(dest => dest.UserPermit, opt => opt.MapFrom(src => src.UserPermit))
                .ForMember(dest => dest.UpdatedVehicle, opt => opt.MapFrom(src => src.UpdatedVehicle));

            CreateMap<AccessLog, AccessLogDto>()
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.UserPermit.Vehicle.PlateNumber))
                .ForMember(dest => dest.LogType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.PermitName, opt => opt.MapFrom(src => src.UserPermit.Permit.Name));
        }
    }
}

