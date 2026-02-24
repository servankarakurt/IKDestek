using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRSupport.UI.Controllers
{
    public class LeaveRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LeaveRequestController> _logger;

        public LeaveRequestController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LeaveRequestController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        // 1. TÜM İZİN TALEPLERİNİ LİSTELE
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/LeaveRequest/GetAll";
                _logger.LogInformation($"API çağrısı: {apiUrl}");

                var response = await client.GetFromJsonAsync<ApiResult<List<LeaveRequestViewModel>>>(apiUrl);

                return View(response?.Value ?? new List<LeaveRequestViewModel>());
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"API hatası: {ex.Message}");
                ModelState.AddModelError("", "İzin talepleri yüklenirken bir hata oluştu.");
                return View(new List<LeaveRequestViewModel>());
            }
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

            try
            {
                var client = _httpClientFactory.CreateClient();

                var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/LeaveRequest";
                var response = await client.PostAsJsonAsync(apiUrl, model);

                if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "İzin talebi gönderilirken bir hata oluştu.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"İzin talebi oluşturma hatası: {ex.Message}");
                ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyin.");
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + $"/api/LeaveRequest/{id}";

            var response = await client.GetFromJsonAsync<ApiResult<UpdateLeaveRequestViewModel>>(apiUrl);

            if (response == null || !response.IsSuccess || response.Value == null) return NotFound();

            return View(response.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateLeaveRequestViewModel model)
        {
            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("", "Başlangıç tarihi bitiş tarihinden büyük veya eşit olamaz.");
                return View(model);
            }

            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/LeaveRequest/update";

            try
            {
                var response = await client.PutAsJsonAsync(apiUrl, model);

                if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "İzin talebi güncellenirken bir hata oluştu.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"İzin talebi güncelleme hatası: {ex.Message}");
                ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyin.");
            }

            return View(model);
        }
    }
}
