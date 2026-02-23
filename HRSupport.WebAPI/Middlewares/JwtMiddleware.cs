namespace HRSupport.WebAPI.Middlewares
{
    using global::HRSupport.Application.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    namespace HRSupport.WebAPI.Middlewares
    {
        public class JwtMiddleware
        {
            private readonly RequestDelegate _next;

            public JwtMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context, ITokenService tokenService)
            {
                var endpoint = context.GetEndpoint();

                if (endpoint != null)
                {
                    var hasAuthorizeAttribute = endpoint.Metadata.GetMetadata<IAuthorizeData>() != null;
                    var hasAllowAnonymousAttribute = endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null;

                    if (hasAuthorizeAttribute && !hasAllowAnonymousAttribute)
                    {
                        // DAHA GÜVENLİ TOKEN OKUMA YÖNTEMİ
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                        {
                            // "Bearer " kelimesinden sonrasını (sadece token metnini) alıyoruz
                            var token = authHeader.Substring("Bearer ".Length).Trim();

                            AttachUserToContext(context, tokenService, token);
                        }
                    }
                }

                await _next(context);
            }

            private void AttachUserToContext(HttpContext context, ITokenService tokenService, string token)
            {
                try
                {
                    if (tokenService.ValidateToken(token, out var jwtToken) && jwtToken != null)
                    {
                        var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, "JwtCustomAuth");
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        context.User = claimsPrincipal;

                        // Opsiyonel: UserId'yi kolay erişim için Items içine ekleme
                        var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                        context.Items["UserId"] = userId;
                    }
                }
                catch
                {
                    // Token doğrulanamadı (Süresi dolmuş veya imza hatalı)
                    // Hata fırlatmanıza gerek yok, context.User boş kalacağı için ASP.NET Core 401 Unauthorized dönecektir.
                }
            }
        }
    }
}
