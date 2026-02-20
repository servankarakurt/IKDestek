using HRSupport.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace HRSupport.UI.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EmployeeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // 1. LİSTELEME
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/GetAllEmployees";
            var response = await client.GetFromJsonAsync<ApiResponse<List<EmployeeViewModel>>>(apiUrl);

            return View(response?.Value ?? new List<EmployeeViewModel>());
        }

        // 2. EKLEME (SAYFA)
        [HttpGet]
        public IActionResult Create() => View();

        // 3. EKLEME (İŞLEM)
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee/create";
            var response = await client.PostAsJsonAsync(apiUrl, model);

            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Çalışan eklenirken bir hata oluştu.");
            return View(model);
        }
    }
}