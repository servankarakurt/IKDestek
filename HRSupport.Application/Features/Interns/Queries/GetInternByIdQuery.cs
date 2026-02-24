using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Interns.Queries
{
    public class GetInternByIdQuery : IRequest<Result<InternDto>>
    {
        public int Id { get; }

        public GetInternByIdQuery(int id)
        {
            Id = id;
        }
    }
}

