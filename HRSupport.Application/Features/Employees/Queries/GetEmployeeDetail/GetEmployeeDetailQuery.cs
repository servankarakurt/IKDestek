using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Employees.Queries.GetEmployeeDetail
{
    public class GetEmployeeDetailQuery : IRequest<Result<EmployeeDetailDto>>
    {
        public int Id { get; set; }
        public GetEmployeeDetailQuery(int id) { Id = id; }
    }
}
