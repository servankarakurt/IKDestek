using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Commands
{
    public class UpdateLeaveRequestHandler : IRequestHandler<UpdateLeaveRequestCommand, Result<bool>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public UpdateLeaveRequestHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<Result<bool>> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null)
                return Result<bool>.Failure("İzin talebi bulunamadı.");
            if (request.StartDate > request.EndDate)
                return Result<bool>.Failure("Başlangıç tarihi bitiş tarihinden büyük olamaz.");

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
