using AutoMapper;
using HRSupport.Application.DTOs;
using HRSupport.Application.Features.Employees.Commands;
using HRSupport.Application.Features.Interns.Commands.CreateIntern;
using HRSupport.Domain.Entities;

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

            // Employee -> EmployeeDto (FullName domain'de; IsActive yok, varsayılan true)
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => (int)src.Department))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => (int)src.Roles))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            // Intern -> InternDto
            CreateMap<Intern, InternDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor != null ? $"{src.Mentor.FirstName} {src.Mentor.LastName}" : "Atanmadı"));

            // LeaveRequest -> LeaveRequestDto (domain Type/Status enum ile DTO uyumlu)
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}