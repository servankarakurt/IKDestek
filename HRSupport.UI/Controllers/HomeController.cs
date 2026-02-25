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
            var client = _httpClientFactory.CreateClient("ApiWithAuth");
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Dashboard/stats";

            try
            {
                var response = await client.GetFromJsonAsync<DashboardStatsViewModel>(apiUrl);
                if (response != null)
                {
                    return View(response); 
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