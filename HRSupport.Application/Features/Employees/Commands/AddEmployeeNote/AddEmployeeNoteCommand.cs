using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class AddEmployeeNoteCommand : IRequest<Result<int>>
    {
        public int EmployeeId { get; set; }
        public string NoteText { get; set; } = string.Empty;
    }
}
