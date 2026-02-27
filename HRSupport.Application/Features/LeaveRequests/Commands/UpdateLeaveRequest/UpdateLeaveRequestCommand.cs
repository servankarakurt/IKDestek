using HRSupport.Application.Common;
using HRSupport.Domain.Enum;
using MediatR;
using System;

namespace HRSupport.Application.Features.LeaveRequests.Commands.UpdateLeaveRequest
{
    public class UpdateLeaveRequestCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType Type { get; set; }
        public LeaveStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
