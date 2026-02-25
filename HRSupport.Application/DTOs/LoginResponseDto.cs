namespace HRSupport.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty; // "Employee" | "Intern"
        public bool MustChangePassword { get; set; }
    }
}
