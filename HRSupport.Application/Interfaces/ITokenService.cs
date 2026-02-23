using HRSupport.Domain.Entites;
using System.IdentityModel.Tokens.Jwt; // Bu kütüphane out parametresi için şart

namespace HRSupport.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token, out JwtSecurityToken? jwt);
    }
}