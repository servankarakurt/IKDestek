using HRSupport.Application.Common;
using HRSupport.Domain.Enum;
using MediatR;

namespace HRSupport.Application.Features.LeaveRequests.Commands.UpdateLeaveRequestStatus
{
    public class UpdateLeaveRequestStatusCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public LeaveStatus NewStatus { get; set; }
        public string? RejectReason { get; set; }
    }
}
