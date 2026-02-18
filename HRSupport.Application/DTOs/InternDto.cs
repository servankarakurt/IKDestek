using System;

namespace HRSupport.Application.DTOs
{
    public class InternDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? University { get; set; }
        public string? Major { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? MentorName { get; set; } 
    }
}