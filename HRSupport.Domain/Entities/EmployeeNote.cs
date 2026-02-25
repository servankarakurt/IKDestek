using HRSupport.Domain.Common;

namespace HRSupport.Domain.Entities
{
    /// <summary>HR veya Yönetici tarafından çalışana girilen basit notlar.</summary>
    public class EmployeeNote : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public string NoteText { get; set; } = string.Empty;
        /// <summary>Notu giren kullanıcı (Employee/User id).</summary>
        public int CreatedByUserId { get; set; }
        /// <summary>Notu giren kişi adı (görüntüleme için).</summary>
        public string? CreatedByUserName { get; set; }
    }
}
