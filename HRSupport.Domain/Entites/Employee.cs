using HRSupport.Domain.Common;
using System;

namespace HRSupport.Domain.Entites
{
    public class Employee: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public Department Deparment { get; set; }
        public Roles Roles { get; set; }

        public String fullname => $"{FirstName} {LastName}";
    }
}