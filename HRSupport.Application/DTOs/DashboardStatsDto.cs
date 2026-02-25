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
        /// <summary>Departman bazında çalışan sayısı (Admin/IK: tümü; Yönetici: sadece kendi birimi).</summary>
        public List<DepartmentCountDto> DepartmentBreakdown { get; set; } = new List<DepartmentCountDto>();
        /// <summary>Yönetici anasayfasında "birimim" başlıkları için true.</summary>
        public bool IsManagerView { get; set; }
        /// <summary>Yönetici birim adı (sadece IsManagerView true ise dolu).</summary>
        public string? ManagerDepartmentName { get; set; }
    }

    public class DepartmentCountDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}