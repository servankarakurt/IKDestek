using HRSupport.Application.Common;
using HRSupport.Domain.Enum;
using MediatR;

namespace HRSupport.Application.Features.Employees.Commans
{
    public class UpdateLeaveRequestStatusCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public LeaveStatus NewStatus { get; set; } // Onaylandı veya Reddedildi
        public string? RejectReason { get; set; } // Opsiyonel reddetme açıklaması
    }
}