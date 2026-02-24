using HRSupport.Domain.Common;
using HRSupport.Domain.Enum;
using System;

namespace HRSupport.Domain.Entites
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public Department Department { get; set; }
        public Roles Roles { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public bool MustChangePassword { get; set; } = false;
        public string Fullname => $"{FirstName} {LastName}";
    }
}