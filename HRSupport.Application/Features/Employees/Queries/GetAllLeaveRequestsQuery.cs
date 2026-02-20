using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetAllLeaveRequestsQuery : IRequest<Result<IEnumerable<LeaveRequestDto>>>
    {
        public string UserId { get; set; } // İstediği yapan kişinin ID'si
        public string Role { get; set; }   // İstediği yapan kişinin Rolü
    }
}