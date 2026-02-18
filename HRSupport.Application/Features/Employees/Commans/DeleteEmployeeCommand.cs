using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Employees.Commans
{
    public class DeleteEmployeeCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public DeleteEmployeeCommand(int id)
        {
            Id = id;
        }
    }
}