using HRSupport.Domain.Common;
using System;

namespace HRSupport.Domain.Entities
{
    public class Intern : BaseEntity
    {
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
        public Employee? Mentor { get; set; }

        // Authentication-related fields for intern
        public string PasswordHash { get; set; } = string.Empty;
        public bool MustChangePassword { get; set; } = false;

        public string FullName => $"{FirstName} {LastName}";
    }
}