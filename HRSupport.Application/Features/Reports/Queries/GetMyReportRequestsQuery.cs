using HRSupport.Application.Common;
using HRSupport.Domain.Entites;
using MediatR;

namespace HRSupport.Application.Features.Reports.Queries
{
    public class GetMyReportRequestsQuery : IRequest<Result<IEnumerable<WeeklyReportRequest>>>
    {
        public int EmployeeId { get; set; }
    }
}
