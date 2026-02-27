using HRSupport.Application.Common;
using MediatR;
using System;

namespace HRSupport.Application.Features.Interns.Commands.AddMentorNote
{
    public class AddMentorNoteCommand : IRequest<Result<int>>
    {
        public int InternId { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public DateTime? NoteDate { get; set; }
    }
}
