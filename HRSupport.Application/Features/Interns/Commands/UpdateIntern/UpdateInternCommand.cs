using HRSupport.Application.Common;
using MediatR;
using System;

namespace HRSupport.Application.Features.Interns.Commands.UpdateIntern
{
    public class UpdateInternCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? University { get; set; }
        public string? Major { get; set; }
        public int Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MentorId { get; set; }
    }
}

