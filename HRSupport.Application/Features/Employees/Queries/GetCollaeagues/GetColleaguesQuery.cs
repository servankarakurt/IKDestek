using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Employees.Queries.GetCollaeagues
{
    public class GetColleaguesQuery : IRequest<Result<IEnumerable<EmployeeDto>>>
    {
    }
}
