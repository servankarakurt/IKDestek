using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public UpdateEmployeeHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<int>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(request.Id);

            if (existingEmployee == null)
            {
                return Result<int>.Failure("Güncellenmek istenen çalışan bulunamadı.");
            }

            existingEmployee.FirstName = request.FirstName;
            existingEmployee.LastName = request.LastName;
            existingEmployee.Email = request.Email;
            existingEmployee.Phone = request.Phone;
            existingEmployee.CardID = request.CardID;
            existingEmployee.BirthDate = request.BirthDate;
            existingEmployee.StartDate = request.StartDate;

            // Hem Employee entity'sinde hem de Command'da artık isim 'Department'
            existingEmployee.Department = request.Department;
            existingEmployee.Roles = request.Roles;
            existingEmployee.TCKN = request.TCKN;

            await _employeeRepository.UpdateAsync(existingEmployee);

            return Result<int>.Success(existingEmployee.Id, "Çalışan bilgileri başarıyla güncellendi.");
        }
    }
}