using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Interns.Commands
{
    public class DeleteInternCommand : IRequest<Result<int>>
    {
        public int Id { get; }

        public DeleteInternCommand(int id)
        {
            Id = id;
        }
    }
}

