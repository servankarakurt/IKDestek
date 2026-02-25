using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Interns.Queries
{
    public class GetMenteesQuery : IRequest<Result<IEnumerable<InternDto>>>
    {
    }
}
