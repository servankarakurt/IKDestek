using HRSupport.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace HRSupport.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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

            try
            {
                var response = await client.PostAsJsonAsync(apiUrl, model);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                    return View(model);
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResult<LoginResponseModel>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || !result.IsSuccess || result.Value == null)
                {
                    ModelState.AddModelError("", result?.Error ?? "E-posta veya şifre hatalı.");
                    return View(model);
                }
            var token = result.Value.Token;

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Giriş sırasında token üretilemedi.");
                return View(model);
            }

            Response.Cookies.Append("JwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
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
            catch (JsonException ex)
            {
                ModelState.AddModelError("", "Sunucu ile bağlantı sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.");
                return View(model);
            }
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

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Auth/change-password";

            try
            {
                var response = await client.PostAsJsonAsync(apiUrl, new
                {
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword
                });

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi. Lütfen yeni şifrenizle tekrar giriş yapın.";
                    await HttpContext.SignOutAsync("HRSupportCookie");
                    Response.Cookies.Delete("JwtToken");
                    return RedirectToAction("Login");
                }

                var errorResult = await response.Content.ReadFromJsonAsync<ApiResult<string>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ModelState.AddModelError("", errorResult?.Error ?? "Şifre değiştirilemedi.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Şifre değiştirilemedi. Lütfen daha sonra tekrar deneyiniz.");
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("HRSupportCookie");
            Response.Cookies.Delete("JwtToken");
            return RedirectToAction("Login");
        }
    }
}