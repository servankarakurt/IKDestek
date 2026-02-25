using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.LeaveRequests.Queries
{
    public class GetMyLeaveBalanceQuery : IRequest<Result<int>>
    {
    }
}
