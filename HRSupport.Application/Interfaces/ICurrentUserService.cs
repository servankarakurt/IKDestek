namespace HRSupport.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        string? Role { get; }
        string? UserType { get; }
        int? DepartmentId { get; }
        bool IsAuthenticated { get; }
    }
}
