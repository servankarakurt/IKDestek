using HRSupport.Application.Interfaces;
using System.Security.Claims;

namespace HRSupport.WebAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(id, out var n) ? n : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

        public string? UserType => _httpContextAccessor.HttpContext?.User?.FindFirstValue("user_type");

        public int? DepartmentId
        {
            get
            {
                var dep = _httpContextAccessor.HttpContext?.User?.FindFirstValue("department");
                return int.TryParse(dep, out var n) ? n : null;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}
