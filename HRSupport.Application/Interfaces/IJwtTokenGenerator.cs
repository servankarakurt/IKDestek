namespace HRSupport.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(int userId, string email, string role, string fullName, string userType, int? departmentId = null);
    }
}
