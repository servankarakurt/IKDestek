using HRSupport.Domain.Common;
using HRSupport.Domain.Enum;
using System;

namespace HRSupport.Domain.Entities
{
    public class LeaveApprovalHistory : BaseEntity
    {
        public int LeaveRequestId { get; set; }
        public int ProcessedByUserId { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.Now;
        public LeaveStatus Action { get; set; }
        public string Comments { get; set; } = string.Empty;
    }
}