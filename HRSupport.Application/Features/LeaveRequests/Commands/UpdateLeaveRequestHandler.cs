using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Enum;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Commands
{
    public class UpdateLeaveRequestHandler : IRequestHandler<UpdateLeaveRequestCommand, Result<bool>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUser;

        public UpdateLeaveRequestHandler(
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeeRepository employeeRepository,
            ICurrentUserService currentUser)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null)
                return Result<bool>.Failure("İzin talebi bulunamadı.");
            if (request.StartDate > request.EndDate)
                return Result<bool>.Failure("Başlangıç tarihi bitiş tarihinden büyük olamaz.");

            var role = _currentUser.Role ?? string.Empty;
            var userId = _currentUser.UserId;
            var departmentId = _currentUser.DepartmentId;
            var isPrivileged = role == "Admin" || role == "IK" || role == "Yönetici";

            if (role == "Admin" || role == "IK")
            {
                // Tam yetki
            }
            else if (role == "Yönetici")
            {
                var targetEmployee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
                if (targetEmployee == null || !departmentId.HasValue || (int)targetEmployee.Department != departmentId.Value)
                    return Result<bool>.Failure("Sadece kendi biriminize ait izin taleplerini güncelleyebilirsiniz.");
            }
            else if (role == "Çalışan" || role == "Stajyer")
            {
                if (!userId.HasValue || leaveRequest.EmployeeId != userId.Value)
                    return Result<bool>.Failure("Sadece kendi izin talebinizi güncelleyebilirsiniz.");
            }
            else
            {
                return Result<bool>.Failure("Bu işlem için yetkiniz yok.");
            }

            // Privileged olmayan kullanıcılar başka çalışan adına düzenleyemez ve durum alanını değiştiremez.
            if (!isPrivileged)
            {
                if (request.EmployeeId != leaveRequest.EmployeeId)
                    return Result<bool>.Failure("İzin talebindeki çalışan bilgisi değiştirilemez.");

                request.Status = leaveRequest.Status;
            }

            leaveRequest.EmployeeId = request.EmployeeId;
            leaveRequest.StartDate = request.StartDate;
            leaveRequest.EndDate = request.EndDate;
            leaveRequest.Type = request.Type;
            leaveRequest.Status = request.Status;
            leaveRequest.Description = request.Description;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            return Result<bool>.Success(true, "İzin talebi güncellendi.");
        }
    }
}
