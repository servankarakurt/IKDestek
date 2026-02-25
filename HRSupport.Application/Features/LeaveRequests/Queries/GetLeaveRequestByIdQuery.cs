using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestByIdQuery : IRequest<Result<LeaveRequestDto>>
    {
        public int Id { get; set; }
        public GetLeaveRequestByIdQuery(int id) => Id = id;
    }
}
