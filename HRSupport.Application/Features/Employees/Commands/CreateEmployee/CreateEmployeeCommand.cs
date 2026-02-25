using HRSupport.Application.Common;
using HRSupport.Domain.Enum;
using MediatR;
using System;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class CreateEmployeeCommand : IRequest<Result<int>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public Department Department { get; set; }
        public Roles Roles { get; set; }
    }
}
