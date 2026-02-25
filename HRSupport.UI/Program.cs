using HRSupport.UI.Filters;
using HRSupport.UI.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<RequireLoginFilter>();
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();
builder.Services.AddTransient<BearerTokenHandler>();
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:5107";
builder.Services.AddHttpClient("ApiWithAuth", c =>
{
    c.BaseAddress = new Uri(apiBaseUrl + "/");
    c.Timeout = TimeSpan.FromSeconds(30);
})
    .AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// #region agent log
try { var logPath = Path.Combine(Directory.GetCurrentDirectory(), "debug-6a60d2.log"); File.AppendAllText(logPath, "{\"sessionId\":\"6a60d2\",\"message\":\"UI started\",\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n"); } catch { }
// #endregion


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();