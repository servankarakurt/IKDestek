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
builder.Services.AddHttpClient("ApiWithAuth")
    .AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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