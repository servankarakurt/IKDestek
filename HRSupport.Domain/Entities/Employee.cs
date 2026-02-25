using HRSupport.Domain.Common;
using HRSupport.Domain.Enum;
using System;

namespace HRSupport.Domain.Entities
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
        /// <summary>TC Kimlik No (opsiyonel).</summary>
        public string? TCKN { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}