using HRSupport.Domain.Common;

namespace HRSupport.Domain.Entites
{
    public class EmployeeLeaveBalance : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int RemainingAnnualLeaveDays { get; set; } 

        
    }
}