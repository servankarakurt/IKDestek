using HRSupport.Domain.Common;
using HRSupport.Domain.Enum;
using System;

namespace HRSupport.Domain.Entities
{
    public class LeaveRequest : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType Type { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Beklemede;
        public string Description { get; set; } = string.Empty;
        public int RequestedDays => (EndDate - StartDate).Days + 1;
        public bool IsUrgentWithoutBalance { get; set; } = false;
        public virtual ICollection<LeaveApprovalHistory> ApprovalHistories { get; set; } = new List<LeaveApprovalHistory>();
    }
}