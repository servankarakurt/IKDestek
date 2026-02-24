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
            // NOT: Auth sistemi kurulduğunda buradaki ID'yi sisteme giren kişiden alacağız.
            // Örn: var currentEmployeeId = int.Parse(User.FindFirst("Id").Value);
            int currentEmployeeId = 1; // Şimdilik test için Sabit 1 numaralı çalışanı kullanıyoruz.

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/LeaveRequest/GetAll";

            var model = new EmployeeDashboardViewModel();

            try
            {
                var response = await client.GetFromJsonAsync<ApiResult<List<LeaveRequestViewModel>>>(apiUrl);

                if (response != null && response.IsSuccess && response.Value != null)
                {
                    // Sadece sisteme giren çalışanın (ID=1) izinlerini filtrele
                    var myRequests = response.Value.Where(x => x.EmployeeId == currentEmployeeId).ToList();

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