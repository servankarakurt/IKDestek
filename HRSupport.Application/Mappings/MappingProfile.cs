using AutoMapper;
using HRSupport.Application.DTOs;
using HRSupport.Domain.Entites;

namespace HRSupport.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee -> EmployeeDto çevirisi
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.ToString()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            // Intern -> InternDto çevirisi
            CreateMap<Intern, InternDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor != null ? $"{src.Mentor.FirstName} {src.Mentor.LastName}" : "Atanmadı"));

            // LeaveRequest -> LeaveRequestDto çevirisi (src.LeaveType yerine src.Type kullanıldı)
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}