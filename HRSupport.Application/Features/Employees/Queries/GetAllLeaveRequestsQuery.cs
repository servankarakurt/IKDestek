using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HRSupport.Application.Features.Employees.Queries
{
    // Tüm izin taleplerini listeleyecek olan ve geriye LeaveRequestDto listesi dönen sorgumuz
    public class GetAllLeaveRequestsQuery : IRequest<Result<IEnumerable<LeaveRequestDto>>>
    {
    }
}