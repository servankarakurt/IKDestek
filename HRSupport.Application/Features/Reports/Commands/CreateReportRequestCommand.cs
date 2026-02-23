using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Reports.Commands
{
    public class CreateReportRequestCommand : IRequest<Result<int>>
    {
        public int ManagerEmployeeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
}
