using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Linq; 

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
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Auth/login";

            var response = await client.PostAsJsonAsync(apiUrl, model);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseModel>>();
                var token = result?.Value?.Token;

                if (!string.IsNullOrEmpty(token))
                {
                    // Token'ı Cookie'ye kaydet
                    Response.Cookies.Append("JwtToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddHours(1)
                    });

                    // --- Şifre Değiştirme Kontrolü ---
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var changeRequired = jwtToken.Claims.FirstOrDefault(c => c.Type == "IsPasswordChangeRequired")?.Value;

                    if (changeRequired == "True")
                    {
                        TempData["Message"] = "İlk girişiniz olduğu için şifrenizi değiştirmeniz gerekmektedir.";
                        return RedirectToAction("ChangePassword");
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "E-posta veya şifre hatalı.");
            return View(model);
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

            // API'ye sadece mevcut ve yeni şifreyi gönderiyoruz (ID'yi API token'dan alacak)
            var response = await client.PostAsJsonAsync(apiUrl, new
            {
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword
            });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi. Lütfen yeni şifrenizle tekrar giriş yapın.";

                // Şifre değişince eski token geçersiz olacağı için logout yapıp login'e atıyoruz
                Response.Cookies.Delete("JwtToken");
                return RedirectToAction("Login");
            }

            var error = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            ModelState.AddModelError("", error?.Message ?? "Şifre değiştirilemedi.");
            return View(model);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("JwtToken");
            return RedirectToAction("Login");
        }
    }
}