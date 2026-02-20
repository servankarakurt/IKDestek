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
    [Authorize] // Index (listeleme) sayfasını sisteme giren herkes görebilir
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee";
            var response = await client.GetFromJsonAsync<ApiResponse<List<EmployeeViewModel>>>(apiUrl);

            return View(response?.Value ?? new List<EmployeeViewModel>());
        }

        // Sadece yetkili kişiler Ekleme sayfasını görüntüleyebilir
        [HttpGet]
        [Authorize(Roles = "1, 2")]
        public IActionResult Create()
        {
            return View();
        }

        // Sadece yetkili kişiler form verisini POST edebilir
        [HttpPost]
        [Authorize(Roles = "1, 2")]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee/create";
            var response = await client.PostAsJsonAsync(apiUrl, model);

            // İşlem başarılıysa Index (Liste) sayfasına yönlendir (Senin takıldığın sayfaya düşmeme sorununun çözümü burasıdır)
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Personel eklenirken bir hata oluştu.");
            return View(model);
        }
    }
}