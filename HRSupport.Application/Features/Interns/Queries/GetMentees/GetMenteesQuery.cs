using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Interns.Queries.GetMentees
{
    public class GetMenteesQuery : IRequest<Result<IEnumerable<InternDto>>>
    {
    }
}
