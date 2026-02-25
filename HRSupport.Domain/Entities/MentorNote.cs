using HRSupport.Domain.Common;

namespace HRSupport.Domain.Entities
{
    /// <summary>Mentorun stajyer için haftalık/kısa geri bildirim notları.</summary>
    public class MentorNote : BaseEntity
    {
        public int InternId { get; set; }
        public Intern? Intern { get; set; }
        public int? MentorId { get; set; }
        public Employee? Mentor { get; set; }
        public string NoteText { get; set; } = string.Empty;
        /// <summary>Haftalık dönem veya tarih etiketi (opsiyonel).</summary>
        public DateTime? NoteDate { get; set; }
    }
}
