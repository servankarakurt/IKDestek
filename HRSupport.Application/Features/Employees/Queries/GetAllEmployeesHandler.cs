using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery, Result<IEnumerable<EmployeeDto>>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetAllEmployeesHandler(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<EmployeeDto>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();

            // Veritabanından gelen Entity listesini, DTO listesine dönüştürüyoruz
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

            return Result<IEnumerable<EmployeeDto>>.Success(employeeDtos, "Çalışan listesi başarıyla getirildi.");
        }
    }
}