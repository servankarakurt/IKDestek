using System;

namespace HRSupport.Application.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Department string'di, artık int olarak gelecek
        public int Department { get; set; }

        // Edit sayfasının ihtiyaç duyduğu eksik alanları ekliyoruz
        public int Roles { get; set; }
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }

        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }
}