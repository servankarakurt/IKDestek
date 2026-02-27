using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Interns.Queries.GetMyMentor
{
    public class GetMyMentorQuery : IRequest<Result<MentorInfoDto?>>
    {
    }
}
