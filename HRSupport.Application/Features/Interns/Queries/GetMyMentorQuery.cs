using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Interns.Queries
{
    public class GetMyMentorQuery : IRequest<Result<MentorInfoDto?>>
    {
    }
}
