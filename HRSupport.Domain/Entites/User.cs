using HRSupport.Domain.Common;
using HRSupport.Domain.Enum; // Roles enum'unun olduğu yer

namespace HRSupport.Domain.Entites
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Roles Role { get; set; }
    }
}