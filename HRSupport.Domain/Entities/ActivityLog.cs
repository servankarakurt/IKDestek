namespace HRSupport.Domain.Entities
{
    /// <summary>
    /// Giriş, çıkış, şifre değişikliği ve kritik işlemler için audit kaydı.
    /// Hassas veri (şifre, token) asla yazılmaz.
    /// </summary>
    public class ActivityLog
    {
        public int Id { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public int? UserId { get; set; }
        public string? UserType { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? EntityName { get; set; }
        public int? EntityId { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
