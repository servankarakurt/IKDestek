using HRSupport.Domain.Common;

namespace HRSupport.Domain.Entities
{
    public class EmployeeLeaveBalance : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int RemainingAnnualLeaveDays { get; set; } 

        
    }
}