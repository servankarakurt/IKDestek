using HRSupport.Domain.Enum;
using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRSupport.UI.Controllers
{
    // Yalnızca yetkili kullanıcıların erişmesi için ileride buraya [Authorize] ekleyeceğiz.
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DashboardController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
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
                ModelState.AddModelError("", "Veriler çekilirken bir hata oluştu: " + ex.Message);
            }

            // Hata olursa veya veri gelmezse boş model gönder
            return View(new DashboardStatsViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLeaveStatus(int leaveRequestId, LeaveStatus newStatus, string? rejectReason)
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/LeaveRequest/UpdateStatus";

            // API'nin beklediği "UpdateLeaveRequestStatusCommand" modeline uygun anonim bir nesne oluşturuyoruz.
            var command = new
            {
                Id = leaveRequestId,
                NewStatus = newStatus,
                RejectReason = rejectReason
            };

            try
            {
                // API'deki [HttpPut("UpdateStatus")] metoduna veriyi gönderiyoruz
                var response = await client.PutAsJsonAsync(apiUrl, command);

                if (response.IsSuccessStatusCode)
                {
                    // İşlem başarılıysa ekranda yeşil bir mesaj göstereceğiz
                    TempData["SuccessMessage"] = "İzin durumu başarıyla güncellendi!";
                }
                else
                {
                    TempData["ErrorMessage"] = "İzin durumu güncellenirken API tarafında bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bağlantı hatası: " + ex.Message;
            }

            // İşlem bittikten sonra tekrar Dashboard sayfasına (Index) geri dön
            return RedirectToAction("Index");
        }
    }
}