using System;
using HRSupport.Domain.Enum; // Type için bu using'in olduğundan emin ol

namespace HRSupport.Application.DTOs
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;

        // EKSİK OLAN VE EKLENMESİ GEREKEN ALANLAR:
        public LeaveType Type { get; set; }
        public bool IsUrgentWithoutBalance { get; set; }
        /// <summary>Dashboard listesinde göstermek için (API tarafında doldurulur).</summary>
        public string? EmployeeName { get; set; }
    }
}