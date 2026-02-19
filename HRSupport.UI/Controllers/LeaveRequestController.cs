using HRSupport.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace HRSupport.UI.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LeaveRequestController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // 1. TÜM İZİN TALEPLERİNİ LİSTELE
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/LeaveRequest";
            var response = await client.GetFromJsonAsync<ApiResponse<List<LeaveRequestViewModel>>>(apiUrl);

            return View(response?.Data ?? new List<LeaveRequestViewModel>());
        }

        // 2. YENİ İZİN TALEBİ OLUŞTUR (SAYFA)
        [HttpGet]
        public IActionResult Create() => View();

        // 3. YENİ İZİN TALEBİ OLUŞTUR (İŞLEM)
        [HttpPost]
        public async Task<IActionResult> Create(CreateLeaveRequestViewModel model)
        {
            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("", "Başlangıç tarihi bitiş tarihinden büyük veya eşit olamaz.");
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/LeaveRequest";
            var response = await client.PostAsJsonAsync(apiUrl, model);

            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "İzin talebi gönderilirken bir hata oluştu.");
            return View(model);
        }
    }
}