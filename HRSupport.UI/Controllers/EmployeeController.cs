using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HRSupport.UI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EmployeeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee";
            var response = await client.GetFromJsonAsync<ApiResult<List<EmployeeViewModel>>>(apiUrl);

            return View(response?.Value ?? new List<EmployeeViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee/create";

            try
            {
                var response = await client.PostAsJsonAsync(apiUrl, model);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
                    TempData["TempPasswordInfo"] = result?.Error ?? "Personel başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }

                // Hata durumunda detaylı bilgi al
                try
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
                    if (errorResult != null)
                        ModelState.AddModelError("", errorResult.Error ?? $"Bir hata oluştu. ({response.StatusCode})");
                    else
                        ModelState.AddModelError("", $"Sunucu hatası: {response.StatusCode}");
                }
                catch
                {
                    ModelState.AddModelError("", $"Sunucu hatası: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bağlantı hatası: {ex.Message}");
            }

            return View(model);
        }
    }
}
