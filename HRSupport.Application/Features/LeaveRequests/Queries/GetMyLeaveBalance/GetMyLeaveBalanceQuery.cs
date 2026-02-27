using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.LeaveRequests.Queries.GetMyLeaveBalance
{
    public class GetMyLeaveBalanceQuery : IRequest<Result<int>>
    {
    }
}
