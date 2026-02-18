using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery, Result<IEnumerable<Employee>>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetAllEmployeesHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<IEnumerable<Employee>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return Result<IEnumerable<Employee>>.Success(employees, "Çalışan listesi başarıyla getirildi.");
        }
    }
}
