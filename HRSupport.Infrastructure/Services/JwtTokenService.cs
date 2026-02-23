using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRSupport.Infrastructure.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // --- 1. TOKEN OLUŞTURMA METODU (Sizin kodunuz) ---
        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT Secret key is missing in configuration.");

            var issuer = jwtSettings["Issuer"] ?? "HRSupportAPI";
            var audience = jwtSettings["Audience"] ?? "HRSupportUser";

            var expiryMinutesStr = jwtSettings["ExpiryMinutes"];
            var expiryMinutes = !string.IsNullOrEmpty(expiryMinutesStr)
                ? Convert.ToInt32(expiryMinutesStr)
                : 60;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsPasswordChangeRequired", user.IsPasswordChangeRequired.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        // --- 2. YENİ EKLENEN MANUEL TOKEN DOĞRULAMA METODU ---
        public bool ValidateToken(string token, out JwtSecurityToken? jwt)
        {
            jwt = null;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT Secret key is missing in configuration.");

            var issuer = jwtSettings["Issuer"] ?? "HRSupportAPI";
            var audience = jwtSettings["Audience"] ?? "HRSupportUser";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Süresi dolan token'a verilen 5 dakikalık toleransı kaldırır
            };

            try
            {
                // Token çözülmeye ve doğrulanmaya çalışılır
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Başarılı olursa dışarıya fırlatılır
                jwt = (JwtSecurityToken)validatedToken;
                return true;
            }
            catch (Exception)
            {
                // Token bozuksa, imza yanlışsa veya süresi geçmişse buraya düşer
                return false;
            }
        }
    }
}