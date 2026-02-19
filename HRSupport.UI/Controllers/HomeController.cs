using HRSupport.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace HRSupport.UI.Controllers
{
    [Authorize] // Sadece giriş yapanlar ana sayfayı görebilir!
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Dashboard/stats";

            // 1. Giriş yaparken çereze (Cookie) kaydettiğimiz Token'ı okuyoruz
            var token = Request.Cookies["JwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth"); // Token yoksa login'e geri at
            }

            // 2. Token'ı Header'a ekliyoruz (Çilingir maymuncuğumuz)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                // 3. API'ye istek atıyoruz
                var response = await client.GetFromJsonAsync<ApiResponse<DashboardStatsViewModel>>(apiUrl);

                if (response != null && response.IsSuccess)
                {
                    return View(response.Data); // Veriyi View'a gönderiyoruz
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda boş model gönderelim ki sayfa patlamasın
                ModelState.AddModelError("", "Veriler çekilirken bir hata oluştu: " + ex.Message);
            }

            return View(new DashboardStatsViewModel());
        }
    }
}