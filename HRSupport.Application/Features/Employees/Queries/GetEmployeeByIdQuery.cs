using HRSupport.Application.Common;
using HRSupport.Domain.Entites;
using MediatR;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetEmployeeByIdQuery : IRequest<Result<Employee>>
    {
        public int Id { get; set; }

        public GetEmployeeByIdQuery(int id)
        {
            Id = id;
        }
    }
}