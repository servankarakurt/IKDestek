using HRSupport.Domain.Common;

namespace HRSupport.Domain.Entities
{
    /// <summary>Staj süresi boyunca stajyere verilen görev (kısa açıklama).</summary>
    public class InternTask : BaseEntity
    {
        public int InternId { get; set; }
        public Intern? Intern { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
