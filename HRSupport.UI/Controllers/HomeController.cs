using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRSupport.UI.Controllers
{
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
            var role = HttpContext.Session.GetString("Role") ?? "";
            if (role == "Stajyer" || role == "Çalışan")
                return RedirectToAction("Index", "PersonelPanel");

            var client = _httpClientFactory.CreateClient("ApiWithAuth");
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Dashboard/stats";

            try
            {
                var response = await client.GetFromJsonAsync<ApiResult<DashboardStatsViewModel>>(apiUrl);
                if (response?.IsSuccess == true && response.Value != null)
                {
                    return View(response.Value);
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