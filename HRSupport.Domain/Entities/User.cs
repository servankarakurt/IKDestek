using HRSupport.Domain.Common;
using HRSupport.Domain.Enum;

namespace HRSupport.Domain.Entities
{
    /// <summary>
    /// Users tablosu (login için; Admin/IK gibi sistem kullanıcıları migration ile burada seed edilmiş olabilir).
    /// Kolonlar: Id, Email, PasswordHash, Role (int), IsPasswordChangeRequired, CreatedTime, Isactive, IsDeleted
    /// </summary>
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        /// <summary>Veritabanında kolon adı "Role" (tekil).</summary>
        public Roles Role { get; set; }
        public bool IsPasswordChangeRequired { get; set; }
    }
}
