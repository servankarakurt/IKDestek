using HRSupport.Application.Common;
using HRSupport.Domain.Enum;
using MediatR;
using System;

namespace HRSupport.Application.Features.Employees.Commans
{
    public class CreateLeaveRequestCommand : IRequest<Result<int>>
    {
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType Type { get; set; }
        public string? Description { get; set; } 
    }
}