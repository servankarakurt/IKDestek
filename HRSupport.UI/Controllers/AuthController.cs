using HRSupport.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Auth/login";

            // API'ye POST isteği atıyoruz
            var response = await client.PostAsJsonAsync(apiUrl, model);

            if (response.IsSuccessStatusCode)
            {
                // API'den dönen Token'ı okuyoruz (Result<string> döndüğünü varsayıyoruz)
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // Result sınıfınızın içindeki 'data' propertysinden token'ı çekiyoruz
                var token = result.GetProperty("value").GetString();

                if (!string.IsNullOrEmpty(token))
                {
                    // Token'ı sonraki API isteklerinde kullanmak için Cookie'ye şifreli kaydediyoruz
                    Response.Cookies.Append("JwtToken", token, new CookieOptions { HttpOnly = true });

                    // MVC tarafında kullanıcıyı "Giriş Yapmış" olarak işaretliyoruz
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.Email) };
                    var identity = new ClaimsIdentity(claims, "HRSupportCookie");
                    await HttpContext.SignInAsync("HRSupportCookie", new ClaimsPrincipal(identity));

                    return RedirectToAction("Index", "Home"); // Başarılıysa anasayfaya git
                }
            }

            ModelState.AddModelError(string.Empty, "Email veya şifre hatalı.");
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