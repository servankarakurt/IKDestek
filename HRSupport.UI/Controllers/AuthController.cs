using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Login(string? returnUrl = null)
        {
            var role = HttpContext.Session.GetString("Role") ?? "";
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                if (role == "Stajyer" || role == "Çalışan")
                    return RedirectToAction("Index", "PersonelPanel");
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/Auth/login";
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(15);

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync(apiUrl, new { model.Email, model.Password });
            }
            catch (TaskCanceledException)
            {
                ModelState.AddModelError("", "API yanıt vermedi (zaman aşımı). Lütfen WebAPI'nin çalıştığından emin olun: " + apiUrl);
                return View(model);
            }
            catch (TimeoutException)
            {
                ModelState.AddModelError("", "API yanıt vermedi (zaman aşımı). WebAPI projesini başlatıp tekrar deneyin.");
                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "API'ye bağlanılamadı. Lütfen WebAPI projesinin çalıştığından ve adresin doğru olduğundan emin olun.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Bağlantı hatası: " + (ex.Message.Length > 80 ? ex.Message.Substring(0, 80) + "…" : ex.Message));
                return View(model);
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResult<LoginResponseModel>>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (response.IsSuccessStatusCode && result?.IsSuccess == true && result.Value != null)
            {
                var data = result.Value;
                HttpContext.Session.SetString("Token", data.Token);
                HttpContext.Session.SetString("Email", data.Email);
                HttpContext.Session.SetString("Role", data.Role);
                HttpContext.Session.SetInt32("UserId", data.UserId);
                HttpContext.Session.SetString("FullName", data.FullName ?? "");
                HttpContext.Session.SetString("UserType", data.UserType ?? "");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                var role = data.Role ?? "";
                if (role == "Stajyer" || role == "Çalışan")
                    return RedirectToAction("Index", "PersonelPanel");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result?.Error ?? "Giriş başarısız. E-posta veya şifreyi kontrol edin.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

