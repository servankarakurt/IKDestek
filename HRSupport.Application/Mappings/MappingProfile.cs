using AutoMapper;
using HRSupport.Application.DTOs;
using HRSupport.Domain.Entites;
using HRSupport.Domain.Enum;
namespace HRSupport.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee -> EmployeeDto çevirisi (Enum'ı string'e çeviriyoruz)
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.ToString()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            // Intern -> InternDto çevirisi (Mentor'un sadece adını alıyoruz)
            CreateMap<Intern, InternDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor != null ? $"{src.Mentor.FirstName} {src.Mentor.LastName}" : "Atanmadı"));

            // LeaveRequest -> LeaveRequestDto çevirisi
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}