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
            if (IsStajyer()) return RedirectToAction("Index", "PersonelPanel");
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
        public async Task<IActionResult> Create()
        {
            if (IsStajyer()) return RedirectToAction("Index", "PersonelPanel");
            await LoadMentorsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInternViewModel model)
        {
            if (IsStajyer()) return RedirectToAction("Index", "PersonelPanel");
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

            await LoadMentorsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email)
        {
            var role = HttpContext.Session.GetString("Role") ?? "";
            if (role != "Admin" && role != "IK")
                return RedirectToAction(nameof(Index));
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "E-posta adresi gerekli.";
                return RedirectToAction(nameof(Index));
            }
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/Auth/reset-password";
            var response = await SendWithTokenAsync(HttpMethod.Post, apiUrl, JsonContent.Create(new { email = email.Trim() }));
            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResult<string>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (response.IsSuccessStatusCode && result?.IsSuccess == true && !string.IsNullOrEmpty(result.Value))
            {
                TempData["TempPasswordInfo"] = $"Şifre sıfırlandı. Yeni geçici şifre: {result.Value} — Stajyere iletin, ilk girişte değiştirmesi istenecektir.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result?.Error ?? "Şifre sıfırlanamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (IsStajyer() && HttpContext.Session.GetInt32("UserId") != id) return RedirectToAction("Index", "PersonelPanel");
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + $"/api/Intern/{id}/detail";
            var response = await SendWithTokenAsync(HttpMethod.Get, apiUrl);
            if (!response.IsSuccessStatusCode) return NotFound();
            var result = await response.Content.ReadFromJsonAsync<ApiResult<InternDetailViewModel>>(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Value == null) return NotFound();
            await LoadMentorsAsync();
            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTask(int internId, string title, string? description)
        {
            if (IsStajyer()) return RedirectToAction("Index", "PersonelPanel");
            if (string.IsNullOrWhiteSpace(title)) { TempData["ErrorMessage"] = "Görev başlığı zorunludur."; return RedirectToAction(nameof(Detail), new { id = internId }); }
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/Intern/tasks";
            var body = JsonContent.Create(new { internId, title, description });
            var response = await SendWithTokenAsync(HttpMethod.Post, apiUrl, body);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
            if (result?.IsSuccess == true) TempData["SuccessMessage"] = "Görev eklendi.";
            else TempData["ErrorMessage"] = result?.Error ?? "Görev eklenemedi.";
            return RedirectToAction(nameof(Detail), new { id = internId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMentorNote(int internId, string noteText, string? noteDate)
        {
            if (IsStajyer()) return RedirectToAction("Index", "PersonelPanel");
            if (string.IsNullOrWhiteSpace(noteText)) { TempData["ErrorMessage"] = "Not metni zorunludur."; return RedirectToAction(nameof(Detail), new { id = internId }); }
            DateTime? nd = null;
            if (!string.IsNullOrWhiteSpace(noteDate) && DateTime.TryParse(noteDate, out var d)) nd = d;
            var apiUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') + "/api/Intern/mentor-notes";
            var body = JsonContent.Create(new { internId, noteText, noteDate = nd });
            var response = await SendWithTokenAsync(HttpMethod.Post, apiUrl, body);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
            if (result?.IsSuccess == true) TempData["SuccessMessage"] = "Mentor notu eklendi.";
            else TempData["ErrorMessage"] = result?.Error ?? "Not eklenemedi.";
            return RedirectToAction(nameof(Detail), new { id = internId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (IsStajyer()) return RedirectToAction("Index", "PersonelPanel");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateInternViewModel model)
        {
            if (IsStajyer()) return RedirectToAction("Index", "PersonelPanel");
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
            if (IsStajyer()) return RedirectToAction("Index", "PersonelPanel");
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

        private bool IsStajyer() => string.Equals(HttpContext.Session.GetString("Role"), "Stajyer", StringComparison.OrdinalIgnoreCase);
    }
}
