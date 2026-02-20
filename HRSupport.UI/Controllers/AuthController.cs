using HRSupport.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace HRSupport.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpGet("Auth/PersonelGiris")]
        public IActionResult PersonelGiris() => View("Login", new LoginViewModel());

        [HttpGet("Auth/StajyerGiris")]
        public IActionResult StajyerGiris() => View("Login", new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Auth/login";

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync(apiUrl, model);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API bağlantısı başarısız. Login endpoint: {ApiUrl}", apiUrl);
                ModelState.AddModelError("", "Giriş servisine bağlanılamadı. Lütfen WebAPI uygulamasının çalıştığını kontrol edin.");
                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseModel>>();
            var token = result?.Value?.Token;

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Giriş sırasında token üretilemedi.");
                return View(model);
            }

            Response.Cookies.Append("JwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("HRSupportCookie", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            });

            var changeRequired = jwtToken.Claims.FirstOrDefault(c => c.Type == "IsPasswordChangeRequired")?.Value;
            if (string.Equals(changeRequired, "True", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Message"] = "İlk girişiniz olduğu için şifrenizi değiştirmeniz gerekmektedir.";
                return RedirectToAction("ChangePassword");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Yeni şifreler uyuşmuyor.");
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var token = Request.Cookies["JwtToken"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Auth/change-password";
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync(apiUrl, new
                {
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API bağlantısı başarısız. ChangePassword endpoint: {ApiUrl}", apiUrl);
                ModelState.AddModelError("", "Şifre değiştirme servisine bağlanılamadı. Lütfen WebAPI uygulamasını kontrol edin.");
                return View(model);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi. Lütfen yeni şifrenizle tekrar giriş yapın.";
                await HttpContext.SignOutAsync("HRSupportCookie");
                Response.Cookies.Delete("JwtToken");
                return RedirectToAction("Login");
            }

            var error = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            ModelState.AddModelError("", error?.Message ?? "Şifre değiştirilemedi.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("HRSupportCookie");
            Response.Cookies.Delete("JwtToken");
            return RedirectToAction("Login");
        }
    }
}
