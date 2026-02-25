using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Commands
{
    public class UpdateLeaveRequestStatusHandler : IRequestHandler<UpdateLeaveRequestStatusCommand, Result<bool>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUser;

        public UpdateLeaveRequestStatusHandler(
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeeRepository employeeRepository,
            ICurrentUserService currentUser)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(UpdateLeaveRequestStatusCommand request, CancellationToken cancellationToken)
        {
            var role = _currentUser.Role ?? "";
            if (role == "Çalışan" || role == "Stajyer")
                return Result<bool>.Failure("Bu işlem için yetkiniz yok.");

            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null)
                return Result<bool>.Failure("İzin talebi bulunamadı.");

            if (role == "Yönetici" && _currentUser.DepartmentId.HasValue)
            {
                var employee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
                if (employee == null || (int)employee.Department != _currentUser.DepartmentId.Value)
                    return Result<bool>.Failure("Sadece kendi biriminize ait izin taleplerini onaylayabilirsiniz.");
            }

            leaveRequest.Status = request.NewStatus;
            var processedBy = _currentUser.UserId ?? 0;

            var historyRecord = new LeaveApprovalHistory
            {
                LeaveRequestId = leaveRequest.Id,
                ActionDate = DateTime.Now,
                Action = request.NewStatus,
                Comments = request.RejectReason ?? (request.NewStatus == LeaveStatus.Onaylandı ? "İzin Onaylandı." : "Durum Güncellendi."),
                ProcessedByUserId = processedBy
            };
            leaveRequest.ApprovalHistories.Add(historyRecord);
            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            return Result<bool>.Success(true);
        }
    }
}
