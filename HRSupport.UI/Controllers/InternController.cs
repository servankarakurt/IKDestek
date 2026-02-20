using HRSupport.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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

        // Sadece yetkili kişiler Stajyer Ekleme sayfasını görebilir
        [HttpGet]
        [Authorize(Roles = "1, 2")]
        public IActionResult Create()
        {
            return View();
        }

        // Sadece yetkili kişiler formu POST edebilir
        [HttpPost]
        [Authorize(Roles = "1, 2")]
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
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Stajyer eklenirken bir hata oluştu.");
            return View(model);
        }
    }
}