using System;
using System.Collections.Generic;

namespace HRSupport.Application.DTOs
{
    public class InternDetailDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? University { get; set; }
        public string? Major { get; set; }
        public int Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MentorId { get; set; }
        public string? MentorName { get; set; }
        public List<InternTaskDto> Tasks { get; set; } = new();
        public List<MentorNoteDto> MentorNotes { get; set; } = new();
    }

    public class InternTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class MentorNoteDto
    {
        public int Id { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public DateTime? NoteDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? MentorName { get; set; }
    }
}
