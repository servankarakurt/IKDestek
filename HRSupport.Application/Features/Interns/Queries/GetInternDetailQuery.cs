using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Interns.Queries
{
    public class GetInternDetailQuery : IRequest<Result<InternDetailDto>>
    {
        public int Id { get; set; }
        public GetInternDetailQuery(int id) { Id = id; }
    }
}
