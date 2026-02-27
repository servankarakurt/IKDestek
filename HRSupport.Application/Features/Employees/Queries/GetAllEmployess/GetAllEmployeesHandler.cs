using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Enum;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Queries.GetAllEmployess
{
    public class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery, Result<IEnumerable<EmployeeDto>>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public GetAllEmployeesHandler(IEmployeeRepository employeeRepository, IMapper mapper, ICurrentUserService currentUser)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<EmployeeDto>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Employee> employees;
            var role = (_currentUser.Role ?? "").Trim();

            if (role.Contains("Admin", StringComparison.OrdinalIgnoreCase) || string.Equals(role, "IK", StringComparison.OrdinalIgnoreCase))
                employees = await _employeeRepository.GetAllEmployeesAsync();
            else if (role.Contains("Yönetici", StringComparison.OrdinalIgnoreCase) && _currentUser.DepartmentId.HasValue)
                employees = await _employeeRepository.GetByDepartmentAsync((Department)_currentUser.DepartmentId.Value);
            else
                employees = new List<Domain.Entities.Employee>();

            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Result<IEnumerable<EmployeeDto>>.Success(employeeDtos, "Çalışan listesi başarıyla getirildi.");
        }
    }
}