using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HRSupport.Application.Features.LeaveRequests.Queries
{
    public class GetAllLeaveRequestsQuery : IRequest<Result<IEnumerable<LeaveRequestDto>>>
    {
        public string? UserId { get; set; }
        public string? Role { get; set; }
    }
}