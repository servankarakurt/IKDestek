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
                var client = _httpClientFactory.CreateClient("ApiWithAuth");

                var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/LeaveRequest";
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
                var client = _httpClientFactory.CreateClient("ApiWithAuth");

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
            var client = _httpClientFactory.CreateClient("ApiWithAuth");
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

            var client = _httpClientFactory.CreateClient("ApiWithAuth");
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

        [HttpGet]
        public async Task<IActionResult> MyBalance()
        {
            var client = _httpClientFactory.CreateClient("ApiWithAuth");
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/LeaveRequest/my-balance";
            try
            {
                var response = await client.GetFromJsonAsync<ApiResult<int>>(apiUrl);
                var days = (response?.IsSuccess == true && response.Value >= 0) ? response.Value : 0;
                return View(days);
            }
            catch
            {
                return View(0);
            }
        }

        /// <summary>
        /// İzin talebi için yıllık ücretli izin formu yazdırma sayfası.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Print(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiWithAuth");
            var baseUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/');

            var leaveResponse = await client.GetFromJsonAsync<ApiResult<UpdateLeaveRequestViewModel>>(baseUrl + $"/api/LeaveRequest/{id}");
            if (leaveResponse == null || !leaveResponse.IsSuccess || leaveResponse.Value == null)
                return NotFound("İzin talebi bulunamadı.");

            var leave = leaveResponse.Value;
            var empResponse = await client.GetFromJsonAsync<ApiResult<UpdateEmployeeViewModel>>(baseUrl + $"/api/Employee/{leave.EmployeeId}");
            if (empResponse == null || !empResponse.IsSuccess || empResponse.Value == null)
                return NotFound("Çalışan bilgisi bulunamadı.");

            var emp = empResponse.Value;
            var days = (leave.EndDate - leave.StartDate).Days + 1;

            var printModel = new LeaveRequestPrintViewModel
            {
                FormPrintDate = DateTime.Now,
                EmployeeName = $"{emp.FirstName} {emp.LastName}".Trim(),
                Kurum = "Hepiyi Sigorta",
                Sirket = "Hepiyi Sigorta A.Ş.",
                DepartmentName = GetDepartmentName(emp.Department),
                Unvan = "",
                SicilNo = emp.CardID,
                LeaveYear = leave.StartDate.Year,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                RequestedDays = days,
                AddressAndPhone = leave.Description ?? "",
                EmployeeStartDate = emp.StartDate,
                KullanilanIzinGunuDisplay = days.ToString(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"))
            };

            return View(printModel);
        }

        private static string GetDepartmentName(int department)
        {
            return department switch
            {
                1 => "Yazılım",
                2 => "İnsan Kaynakları",
                3 => "Satış",
                4 => "Muhasebe",
                5 => "Pazarlama",
                6 => "Operasyon",
                7 => "Acente",
                _ => ""
            };
        }
    }
}
