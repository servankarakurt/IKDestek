using System;
using System.Collections.Generic;

namespace HRSupport.Application.DTOs
{
    public class EmployeeDetailDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? TCKN { get; set; }
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public int Department { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int Roles { get; set; }
        public string RolesName { get; set; } = string.Empty;
        public List<EmployeeNoteDto> Notes { get; set; } = new();
        public List<LeaveRequestDto> LeaveHistory { get; set; } = new();
    }

    public class EmployeeNoteDto
    {
        public int Id { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
