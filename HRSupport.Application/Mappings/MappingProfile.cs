using AutoMapper;
using HRSupport.Application.DTOs;
using HRSupport.Application.Features.Employees.Commans;
using HRSupport.Application.Features.Interns.Commands;
using HRSupport.Domain.Entites;

namespace HRSupport.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateEmployeeCommand -> Employee çevirisi
            CreateMap<CreateEmployeeCommand, Employee>();

            // CreateInternCommand -> Intern çevirisi
            CreateMap<CreateInternCommand, Intern>();

            // Employee -> EmployeeDto çevirisi
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => (int)src.Department))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => (int)src.Roles))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            // Intern -> InternDto çevirisi
            CreateMap<Intern, InternDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor != null ? $"{src.Mentor.FirstName} {src.Mentor.LastName}" : "Atanmadı"));

            // LeaveRequest -> LeaveRequestDto çevirisi (src.LeaveType yerine src.Type kullanıldı)
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}