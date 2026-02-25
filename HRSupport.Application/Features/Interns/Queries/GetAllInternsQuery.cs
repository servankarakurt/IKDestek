using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HRSupport.Application.Features.Interns.Queries
{
    public class GetAllInternsQuery : IRequest<Result<IEnumerable<InternDto>>>
    {
    }
}