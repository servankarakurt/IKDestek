using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRSupport.UI.Controllers
{
    public class EmployeePanelController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EmployeePanelController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var currentEmployeeId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var client = _httpClientFactory.CreateClient("ApiWithAuth");
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/LeaveRequest";

            var model = new EmployeeDashboardViewModel();

            try
            {
                var response = await client.GetFromJsonAsync<ApiResult<List<LeaveRequestViewModel>>>(apiUrl);

                if (response != null && response.IsSuccess && response.Value != null)
                {
                    // API rol bazlı zaten kendi taleplerini döndürüyor; yine de ID ile filtreleyebiliriz
                    var myRequests = currentEmployeeId > 0
                        ? response.Value.Where(x => x.EmployeeId == currentEmployeeId).ToList()
                        : response.Value;

                    model.MyLeaveRequests = myRequests;
                    model.PendingRequestsCount = myRequests.Count(x => x.Status == 1); // 1: Beklemede
                    model.ApprovedRequestsCount = myRequests.Count(x => x.Status == 3); // 3: Onaylandı
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Veriler çekilirken bir hata oluştu: " + ex.Message);
            }

            return View(model);
        }
    }
}