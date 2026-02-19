using System;
using System.Collections.Generic;
using System.Text;

namespace HRSupport.Application.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalEmployees { get; set; }
        public int TotalInterns { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int ApprovedLeaveRequests { get; set; }
    }
}
