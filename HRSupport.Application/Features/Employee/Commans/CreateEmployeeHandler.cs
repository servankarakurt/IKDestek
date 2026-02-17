using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;

namespace HRSupport.Application.Features.Employee.Commans
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        public CreateEmployeeHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<Result<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                CardID = request.CardID,
                BirthDate = request.BirthDate,
                StartDate = request.StartDate,
                Deparment = request.Deparment,
                Roles = request.Roles
            };
            var newId = await _employeeRepository.AddAsync(employee);
            return Result<int>.Success(employee.Id, "Çalışan başarılı şekilde kaydedildi");

        }
    }
}
