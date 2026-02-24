using System.Collections.Generic;

namespace HRSupport.UI.Models
{
    public class EmployeeDashboardViewModel
    {
        public int RemainingAnnualLeave { get; set; } = 20; 
        public int UsedLeaveDays { get; set; } = 0;

        public int PendingRequestsCount { get; set; }
        public int ApprovedRequestsCount { get; set; }
        public List<LeaveRequestViewModel> MyLeaveRequests { get; set; } = new List<LeaveRequestViewModel>();
    }
}