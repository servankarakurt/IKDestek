namespace HRSupport.Application.Interfaces
{
    /// <summary>
    /// Giriş, çıkış, şifre ve kritik işlem loglarını ActivityLog tablosuna yazar.
    /// Hassas veri (şifre, token) asla geçilmemelidir.
    /// </summary>
    public interface IActivityLogService
    {
        /// <summary>
        /// Bir aktivite kaydı yazar. Ana işlemin transaction'ına bağlı olabilir; hata durumunda çağıran sorumludur.
        /// </summary>
        Task LogAsync(
            int? userId,
            string? userType,
            string action,
            string? entityName,
            int? entityId,
            bool success,
            string? message = null,
            string? ipAddress = null,
            string? userAgent = null,
            CancellationToken cancellationToken = default);
    }
}
