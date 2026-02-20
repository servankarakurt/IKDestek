using HRSupport.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace HRSupport.UI.Controllers
{
    [Authorize]
    public class InternController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public InternController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Intern";
            var response = await client.GetFromJsonAsync<ApiResponse<List<InternViewModel>>>(apiUrl);

            return View(response?.Value ?? new List<InternViewModel>());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateInternViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Intern/create";
            var response = await client.PostAsJsonAsync(apiUrl, model);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();
                TempData["TempPasswordInfo"] = result?.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Stajyer eklenirken bir hata oluştu.");
            return View(model);
        }
    }
}
