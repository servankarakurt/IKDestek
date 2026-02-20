using HRSupport.Domain.Common;
using System;

namespace HRSupport.Domain.Entites
{
    public class WeeklyReportRequest : BaseEntity
    {
        public int ManagerId { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}