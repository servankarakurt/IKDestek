using HRSupport.Application.Common;
using MediatR;
using System;

namespace HRSupport.Application.Features.Reports.Commands
{
    public class CreateReportRequestCommand : IRequest<Result<int>>
    {
        public int ManagerId { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}