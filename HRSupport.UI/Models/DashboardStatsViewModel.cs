using HRSupport.Application.DTOs;

namespace HRSupport.UI.Models
{
    // API'den dönen ana veri
    public class DashboardStatsViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalInterns { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int ApprovedLeaveRequests { get; set; }
        public List<LeaveRequestDto> RecentPendingRequests { get; set; } = new List<LeaveRequestDto>();
    }

    // API'mizin Result<T> yapısını karşılamak için sarmalayıcı sınıf
    public class ApiResponse<T>
    {
        public T? Value { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}