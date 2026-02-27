using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Commands.CreateLeaveRequest
{
    public class CreateLeaveRequestHandler : IRequestHandler<CreateLeaveRequestCommand, Result<int>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUser;

        public CreateLeaveRequestHandler(
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeeRepository employeeRepository,
            ICurrentUserService currentUser)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<int>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var role = _currentUser.Role ?? string.Empty;
            var userId = _currentUser.UserId;
            var departmentId = _currentUser.DepartmentId;

            if (role == "Çalışan" || role == "Stajyer")
            {
                if (!userId.HasValue || request.EmployeeId != userId.Value)
                    return Result<int>.Failure("Sadece kendiniz için izin talebi oluşturabilirsiniz.");
            }
            else if (role == "Yönetici")
            {
                var targetEmployeeForManager = await _employeeRepository.GetByIdAsync(request.EmployeeId);
                if (targetEmployeeForManager == null || !departmentId.HasValue || (int)targetEmployeeForManager.Department != departmentId.Value)
                    return Result<int>.Failure("Sadece kendi biriminiz için izin talebi oluşturabilirsiniz.");
            }
            else if (role != "Admin" && role != "IK")
            {
                return Result<int>.Failure("Bu işlem için yetkiniz yok.");
            }

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
                Description = request.Description ?? string.Empty
            };

            var newId = await _leaveRequestRepository.AddAsync(leaveRequest);
            return Result<int>.Success(newId, "İzin talebi başarıyla oluşturuldu ve onaya sunuldu.");
        }
    }
}
