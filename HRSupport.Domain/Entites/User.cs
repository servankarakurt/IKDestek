using HRSupport.Domain.Common;

namespace HRSupport.Domain.Entites
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Roles Role { get; set; }
        public bool IsPasswordChangeRequired { get; set; } = true;
    }
}