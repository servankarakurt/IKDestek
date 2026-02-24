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

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee";
            try
            {
                var httpResponse = await client.GetAsync(apiUrl);
                var content = await httpResponse.Content.ReadAsStringAsync();
                if (!httpResponse.IsSuccessStatusCode)
                {
                    // Try to show server error detail if any
                    ModelState.AddModelError("", $"Sunucu hatası: {httpResponse.StatusCode}. {content}");
                    return View(new List<EmployeeViewModel>());
                }

                var result = System.Text.Json.JsonSerializer.Deserialize<ApiResult<List<EmployeeViewModel>>>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(result?.Value ?? new List<EmployeeViewModel>());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bağlantı hatası: {ex.Message}");
                return View(new List<EmployeeViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee/create";

            try
            {
                var response = await client.PostAsJsonAsync(apiUrl, model);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<int>>();
                    TempData["TempPasswordInfo"] = result?.Error ?? "Personel başarıyla eklendi.";
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
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + $"/api/Employee/{id}";

            var response = await client.GetFromJsonAsync<ApiResult<UpdateEmployeeViewModel>>(apiUrl);

            if (response == null || !response.IsSuccess || response.Value == null)
            {
                return NotFound();
            }

            return View(response.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee/update";

            try
            {
                var response = await client.PutAsJsonAsync(apiUrl, model);

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
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + $"/api/Employee/delete/{id}";

            try
            {
                var response = await client.DeleteAsync(apiUrl);

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
    }
            
}

