using System.Net.Http.Headers;
using System.Net.Http.Json;
using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRSupport.UI.Controllers
{
    public class InternController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public InternController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Intern") });

            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/Intern";
            var response = await SendWithTokenAsync(HttpMethod.Get, apiUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                TempData["LoginMessage"] = "Oturumunuz sona erdi veya token geçersiz. Tekrar giriş yapın.";
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Intern") });
            }

            var result = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ApiResult<List<InternViewModel>>>()
                : null;
            return View(result?.Value ?? new List<InternViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInternViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/Intern/create";

            try
            {
                var response = await SendWithTokenAsync(HttpMethod.Post, apiUrl, JsonContent.Create(model));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
                    TempData["TempPasswordInfo"] = result?.Error ?? "Stajyer başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }

                // Hata durumunda detaylı bilgi al
                try
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
                    if (errorResult != null)
                        ModelState.AddModelError("", errorResult.Error ?? $"Bir hata oluştu. ({response.StatusCode})");
                    else
                        ModelState.AddModelError("", $"Sunucu hatası: {response.StatusCode}");
                }
                catch
                {
                    ModelState.AddModelError("", $"Sunucu hatası: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bağlantı hatası: {ex.Message}");
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + $"/api/Intern/{id}";
            var response = await SendWithTokenAsync(HttpMethod.Get, apiUrl);
            if (!response.IsSuccessStatusCode)
                return NotFound();
            var result = await response.Content.ReadFromJsonAsync<ApiResult<UpdateInternViewModel>>();
            if (result == null || !result.IsSuccess || result.Value == null)
                return NotFound();

            await LoadMentorsAsync();
            return View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateInternViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/Intern/update";

            try
            {
                var response = await SendWithTokenAsync(HttpMethod.Put, apiUrl, JsonContent.Create(model));

                if (response.IsSuccessStatusCode)
                {
                    TempData["TempPasswordInfo"] = "Stajyer başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bağlantı hatası: {ex.Message}");
            }

            await LoadMentorsAsync();
            return View(model);
        }

        private async Task LoadMentorsAsync()
        {
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/Employee";
            try
            {
                var response = await SendWithTokenAsync(HttpMethod.Get, apiUrl);
                var result = response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ApiResult<List<EmployeeLookupViewModel>>>() : null;
                var mentors = result?.Value ?? new List<EmployeeLookupViewModel>();
                ViewBag.Mentors = new SelectList(mentors, "Id", "FullName");
            }
            catch
            {
                ViewBag.Mentors = new SelectList(Enumerable.Empty<EmployeeLookupViewModel>(), "Id", "FullName");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Print(int id)
        {
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + $"/api/LeaveRequest/{id}";
            try
            {
                var response = await SendWithTokenAsync(HttpMethod.Get, apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<LeaveRequestViewModel>>();
                    if (result != null && result.IsSuccess && result.Value != null)
                        return View(result.Value);
                }
            }
            catch { }

            return NotFound("İzin belgesi bulunamadı.");
        }
    }
}
