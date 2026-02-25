namespace HRSupport.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Role { get; }
        int? DepartmentId { get; }
        bool IsAuthenticated { get; }
    }
}
