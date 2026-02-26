using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestPrintInfoQuery : IRequest<Result<LeaveRequestPrintInfoDto>>
    {
        public int Id { get; set; }
        public GetLeaveRequestPrintInfoQuery(int id) => Id = id;
    }
}
