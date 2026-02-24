using System.Collections.Generic;

namespace HRSupport.Application.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalEmployees { get; set; }
        public int TotalInterns { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int ApprovedLeaveRequests { get; set; }
        public List<LeaveRequestDto> RecentPendingRequests { get; set; } = new List<LeaveRequestDto>();
    }
}