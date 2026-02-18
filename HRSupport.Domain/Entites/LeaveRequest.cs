using HRSupport.Domain.Common;
using HRSupport.Domain.Enum;
using System;

namespace HRSupport.Domain.Entites
{
    public class LeaveRequest : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType Type { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Beklemede; // Varsayılan: Beklemede
        public string Description { get; set; }
    }
}