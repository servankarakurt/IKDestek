using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Commans
{
    public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public DeleteEmployeeHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<int>> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            
            var existingEmployee = await _employeeRepository.GetByIdAsync(request.Id);

            if (existingEmployee == null)
            {
                return Result<int>.Failure("Silinmek istenen çalışan bulunamadı veya zaten silinmiş.");
            }

            await _employeeRepository.DeleteAsync(request.Id);

            return Result<int>.Success(request.Id, "Çalışan başarıyla silindi.");
        }
    }
}