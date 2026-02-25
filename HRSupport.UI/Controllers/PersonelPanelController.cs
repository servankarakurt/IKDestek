using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRSupport.UI.Controllers
{
    public class PersonelPanelController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PersonelPanelController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private async Task<HttpResponseMessage> SendWithTokenAsync(HttpMethod method, string url, HttpContent? content = null)
        {
            var token = HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(method, url) { Content = content };
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await client.SendAsync(request);
        }

        private string BaseUrl => _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') ?? "";

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role") ?? "";
            if (string.Equals(role, "Stajyer", StringComparison.OrdinalIgnoreCase))
                return await StajyerPanel();
            if (string.Equals(role, "Çalışan", StringComparison.OrdinalIgnoreCase))
                return await CalisanPanel();
            return RedirectToAction("Index", "Home");
        }

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private async Task<IActionResult> StajyerPanel()
        {
            var model = new StajyerPanelViewModel();
            try
            {
                var balanceRes = await SendWithTokenAsync(HttpMethod.Get, BaseUrl + "/api/LeaveRequest/my-balance");
                if (balanceRes.IsSuccessStatusCode)
                {
                    var balanceResult = await balanceRes.Content.ReadFromJsonAsync<ApiResult<int>>(JsonOptions);
                    model.KalanIzinGunu = balanceResult?.Value ?? 0;
                }

                var leaveRes = await SendWithTokenAsync(HttpMethod.Get, BaseUrl + "/api/LeaveRequest");
                if (leaveRes.IsSuccessStatusCode)
                {
                    var leaveResult = await leaveRes.Content.ReadFromJsonAsync<ApiResult<List<LeaveRequestViewModel>>>(JsonOptions);
                    var list = leaveResult?.Value ?? new List<LeaveRequestViewModel>();
                    model.Taleplerim = list.OrderByDescending(x => x.StartDate).ToList();
                    model.SonBesTalep = model.Taleplerim.Take(5).ToList();
                }

                var mentorRes = await SendWithTokenAsync(HttpMethod.Get, BaseUrl + "/api/Intern/my-mentor");
                if (mentorRes.IsSuccessStatusCode)
                {
                    var mentorResult = await mentorRes.Content.ReadFromJsonAsync<ApiResult<MentorInfoViewModel>>(JsonOptions);
                    model.Mentor = mentorResult?.Value;
                }
            }
            catch
            {
                ModelState.AddModelError("", "Veriler yüklenirken hata oluştu.");
            }

            return View("StajyerPanel", model);
        }

        private async Task<IActionResult> CalisanPanel()
        {
            var model = new CalisanPanelViewModel();
            try
            {
                var balanceRes = await SendWithTokenAsync(HttpMethod.Get, BaseUrl + "/api/LeaveRequest/my-balance");
                if (balanceRes.IsSuccessStatusCode)
                {
                    var balanceResult = await balanceRes.Content.ReadFromJsonAsync<ApiResult<int>>(JsonOptions);
                    model.KalanIzinGunu = balanceResult?.Value ?? 0;
                }

                var leaveRes = await SendWithTokenAsync(HttpMethod.Get, BaseUrl + "/api/LeaveRequest");
                if (leaveRes.IsSuccessStatusCode)
                {
                    var leaveResult = await leaveRes.Content.ReadFromJsonAsync<ApiResult<List<LeaveRequestViewModel>>>(JsonOptions);
                    var list = leaveResult?.Value ?? new List<LeaveRequestViewModel>();
                    model.Taleplerim = list.OrderByDescending(x => x.StartDate).ToList();
                    model.SonBesTalep = model.Taleplerim.Take(5).ToList();
                }

                var colleaguesRes = await SendWithTokenAsync(HttpMethod.Get, BaseUrl + "/api/Employee/colleagues");
                if (colleaguesRes.IsSuccessStatusCode)
                {
                    var colleaguesResult = await colleaguesRes.Content.ReadFromJsonAsync<ApiResult<List<ColleagueViewModel>>>(JsonOptions);
                    model.BirimArkadaslari = colleaguesResult?.Value ?? new List<ColleagueViewModel>();
                }

                var menteesRes = await SendWithTokenAsync(HttpMethod.Get, BaseUrl + "/api/Intern/mentees");
                if (menteesRes.IsSuccessStatusCode)
                {
                    var menteesResult = await menteesRes.Content.ReadFromJsonAsync<ApiResult<List<MenteeViewModel>>>(JsonOptions);
                    model.MenteeListesi = menteesResult?.Value ?? new List<MenteeViewModel>();
                }
            }
            catch
            {
                ModelState.AddModelError("", "Veriler yüklenirken hata oluştu.");
            }

            return View("CalisanPanel", model);
        }
    }
}
