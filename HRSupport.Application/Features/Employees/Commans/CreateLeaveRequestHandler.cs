using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HRSupport.Domain.Enum;

namespace HRSupport.Application.Features.Commands
    {
        public class CreateLeaveRequestHandler : IRequestHandler<CreateLeaveRequestCommand, Result<int>>
        {
            private readonly ILeaveRequestRepository _leaveRequestRepository;
            private readonly IEmployeeRepository _employeeRepository;

            public CreateLeaveRequestHandler(ILeaveRequestRepository leaveRequestRepository, IEmployeeRepository employeeRepository)
            {
                _leaveRequestRepository = leaveRequestRepository;
                _employeeRepository = employeeRepository;
            }

            public async Task<Result<int>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
            {
               
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
                if (employee == null) return Result<int>.Failure("Personel bulunamadı.");

                if (request.StartDate > request.EndDate) return Result<int>.Failure("Başlangıç tarihi bitişten büyük olamaz.");

                var leaveRequest = new LeaveRequest
                {
                    EmployeeId = request.EmployeeId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Type = request.Type,
                    Status = LeaveStatus.Beklemede, 
                    Description = request.Description
                };

                var newId = await _leaveRequestRepository.AddAsync(leaveRequest);
                return Result<int>.Success(newId, "İzin talebi başarıyla oluşturuldu ve onaya sunuldu.");
            }
        }
    }
}
}
