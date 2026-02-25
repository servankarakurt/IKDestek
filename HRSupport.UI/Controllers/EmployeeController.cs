using System.Net.Http.Headers;
using HRSupport.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HRSupport.UI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EmployeeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string BaseUrl => _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') ?? "";

        private HttpRequestMessage CreateRequest(HttpMethod method, string path, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, BaseUrl + path) { Content = content };
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }

        public async Task<IActionResult> Index()
        {
            if (IsCalisan()) return RedirectToAction("Index", "PersonelPanel");
            var client = _httpClientFactory.CreateClient();
            try
            {
                var request = CreateRequest(HttpMethod.Get, "/api/Employee");
                var httpResponse = await client.SendAsync(request);
                var content = await httpResponse.Content.ReadAsStringAsync();
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var msg = (int)httpResponse.StatusCode == 401
                        ? "Oturum süresi dolmuş veya geçersiz. Lütfen tekrar giriş yapın."
                        : (int)httpResponse.StatusCode == 403
                            ? "Bu sayfaya erişim yetkiniz yok."
                            : $"Sunucu hatası: {httpResponse.StatusCode}. {content}";
                    ModelState.AddModelError("", msg);
                    return View(new List<EmployeeViewModel>());
                }
                var opt = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = System.Text.Json.JsonSerializer.Deserialize<ApiResult<List<EmployeeViewModel>>>(content, opt);
                var list = result?.Value ?? new List<EmployeeViewModel>();
                if (result != null && result.Value == null && content.Length > 5)
                {
                    var preview = content.Length > 400 ? content.Substring(0, 400) + "..." : content;
                    ModelState.AddModelError("", $"API yanıtında 'value' bulunamadı veya null. Ham yanıt: {preview}");
                }
                return View(list);
            }
            catch (Exception ex)
            {
                var baseUrl = BaseUrl;
                ModelState.AddModelError("", $"API'ye bağlanılamadı ({ex.Message}). WebAPI'nin {baseUrl} adresinde çalıştığından emin olun. Detay için tarayıcıda /Employee/Diagnostic adresine gidin.");
                return View(new List<EmployeeViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (IsCalisan()) return RedirectToAction("Index", "PersonelPanel");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (IsCalisan()) return RedirectToAction("Index", "PersonelPanel");
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            try
            {
                var content = System.Net.Http.Json.JsonContent.Create(model);
                var request = CreateRequest(HttpMethod.Post, "/api/Employee/create", content);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var jsonOpt = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>(jsonOpt);
                    TempData["TempPasswordInfo"] = result?.Error ?? "Personel başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    var jsonOpt = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResult = await response.Content.ReadFromJsonAsync<ApiResult<int>>(jsonOpt);
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
            if (IsCalisan()) return RedirectToAction("Index", "PersonelPanel");
            var client = _httpClientFactory.CreateClient();
            var request = CreateRequest(HttpMethod.Get, $"/api/Employee/{id}");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return NotFound();
            var result = await response.Content.ReadFromJsonAsync<ApiResult<UpdateEmployeeViewModel>>(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result == null || !result.IsSuccess || result.Value == null) return NotFound();
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (IsCalisan()) return RedirectToAction("Index", "PersonelPanel");
            var client = _httpClientFactory.CreateClient();
            var request = CreateRequest(HttpMethod.Get, $"/api/Employee/{id}/detail");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return NotFound();
            var jsonOpt = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = await response.Content.ReadFromJsonAsync<ApiResult<EmployeeDetailViewModel>>(jsonOpt);
            if (result?.Value == null) return NotFound();
            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(int employeeId, string noteText)
        {
            if (IsCalisan()) return RedirectToAction("Index", "PersonelPanel");
            if (string.IsNullOrWhiteSpace(noteText)) { TempData["ErrorMessage"] = "Not metni boş olamaz."; return RedirectToAction(nameof(Detail), new { id = employeeId }); }
            var client = _httpClientFactory.CreateClient();
            var body = System.Net.Http.Json.JsonContent.Create(new { employeeId, noteText });
            var request = CreateRequest(HttpMethod.Post, "/api/Employee/notes", body);
            var response = await client.SendAsync(request);
            var jsonOpt = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>(jsonOpt);
            if (result?.IsSuccess == true)
                TempData["SuccessMessage"] = "Not eklendi.";
            else
                TempData["ErrorMessage"] = result?.Error ?? "Not eklenemedi.";
            return RedirectToAction(nameof(Detail), new { id = employeeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEmployeeViewModel model)
        {
            if (IsCalisan()) return RedirectToAction("Index", "PersonelPanel");
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient();
            try
            {
                var content = System.Net.Http.Json.JsonContent.Create(model);
                var request = CreateRequest(HttpMethod.Put, "/api/Employee/update", content);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["TempPasswordInfo"] = "Personel başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bağlantı hatası: {ex.Message}");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (IsCalisan()) return RedirectToAction("Index", "PersonelPanel");
            var client = _httpClientFactory.CreateClient();
            try
            {
                var request = CreateRequest(HttpMethod.Delete, $"/api/Employee/delete/{id}");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Personel başarıyla silindi." });
                }

                return Json(new { success = false, message = "Silme işlemi başarısız oldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Bağlantı hatası: {ex.Message}" });
            }
        }

        /// <summary>Tanı: Session ve API yanıtını düz metin olarak döndürür. Admin girişi yaptıktan sonra /Employee/Diagnostic adresine gidin.</summary>
        [HttpGet]
        public async Task<IActionResult> Diagnostic()
        {
            var role = HttpContext.Session.GetString("Role");
            var token = HttpContext.Session.GetString("Token");
            var baseUrl = BaseUrl;
            var statusCode = 0;
            var responsePreview = "";
            try
            {
                var request = CreateRequest(HttpMethod.Get, "/api/Employee");
                var client = _httpClientFactory.CreateClient();
                var resp = await client.SendAsync(request);
                statusCode = (int)resp.StatusCode;
                var body = await resp.Content.ReadAsStringAsync();
                responsePreview = body.Length > 800 ? body.Substring(0, 800) + "..." : body;
            }
            catch (Exception ex)
            {
                responsePreview = "Exception: " + ex.Message;
            }
            return Content(
                $"Role=[{role ?? "(null)"}]\nHasToken={!string.IsNullOrEmpty(token)}\nTokenLength={token?.Length ?? 0}\nBaseUrl={baseUrl}\nApiStatusCode={statusCode}\n\nResponsePreview:\n{responsePreview}",
                "text/plain");
        }

        private bool IsCalisan() => string.Equals(HttpContext.Session.GetString("Role"), "Çalışan", StringComparison.OrdinalIgnoreCase);
    }
}

