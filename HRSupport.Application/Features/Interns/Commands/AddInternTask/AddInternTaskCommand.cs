using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Interns.Commands.AddInternTask
{
    public class AddInternTaskCommand : IRequest<Result<int>>
    {
        public int InternId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
