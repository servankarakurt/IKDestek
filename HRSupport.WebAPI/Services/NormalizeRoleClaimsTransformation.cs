using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace HRSupport.WebAPI.Services
{
    /// <summary>JWT'deki rol adını [Authorize(Roles = "Admin,IK,Yönetici")] ile uyumlu hale getirir.</summary>
    public class NormalizeRoleClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var role = principal.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(role)) return Task.FromResult(principal);
            if (principal.IsInRole("Admin") || principal.IsInRole("IK") || principal.IsInRole("Yönetici"))
                return Task.FromResult(principal);

            var normalized = role.Trim();
            var identity = (ClaimsIdentity)principal.Identity!;
            if (identity == null) return Task.FromResult(principal);

            var newIdentity = new ClaimsIdentity(identity.Claims, identity.AuthenticationType, identity.NameClaimType, identity.RoleClaimType);
            if (normalized.Contains("Admin", StringComparison.OrdinalIgnoreCase))
                newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, "Admin"));
            if (normalized.Equals("IK", StringComparison.OrdinalIgnoreCase))
                newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, "IK"));
            if (normalized.Contains("Yönetici", StringComparison.OrdinalIgnoreCase))
                newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, "Yönetici"));

            return Task.FromResult(new ClaimsPrincipal(newIdentity));
        }
    }
}
