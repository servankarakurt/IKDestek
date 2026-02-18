using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, Result<Employee>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeeByIdHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<Employee>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.Id);

            
            if (employee == null)
            {
                return Result<Employee>.Failure("Belirtilen Id'ye sahip çalışan bulunamadı.");
            }

            return Result<Employee>.Success(employee, "Çalışan başarıyla getirildi.");
        }
    }
}