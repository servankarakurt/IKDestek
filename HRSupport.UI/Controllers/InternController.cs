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

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Intern";
            var response = await client.GetFromJsonAsync<ApiResult<List<InternViewModel>>>(apiUrl);

            return View(response?.Value ?? new List<InternViewModel>());
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

            var client = _httpClientFactory.CreateClient();

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Intern/create";

            try
            {
                var response = await client.PostAsJsonAsync(apiUrl, model);

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
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + $"/api/Intern/{id}";

            var response = await client.GetFromJsonAsync<ApiResult<UpdateInternViewModel>>(apiUrl);

            if (response == null || !response.IsSuccess || response.Value == null)
            {
                return NotFound();
            }

            await LoadMentorsAsync();
            return View(response.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateInternViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Intern/update";

            try
            {
                var response = await client.PutAsJsonAsync(apiUrl, model);

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
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/api/Employee";

            try
            {
                var response = await client.GetFromJsonAsync<ApiResult<List<EmployeeLookupViewModel>>>(apiUrl);
                var mentors = response?.Value ?? new List<EmployeeLookupViewModel>();

                ViewBag.Mentors = new SelectList(mentors, "Id", "FullName");
            }
            catch
            {
                ViewBag.Mentors = new SelectList(Enumerable.Empty<EmployeeLookupViewModel>(), "Id", "FullName");
            }
        }
    }
}
