var builder = WebApplication.CreateBuilder(args);

// MVC Controller ve View'ları ekle
builder.Services.AddControllersWithViews();

// API ile konuşmak için HttpClient'ı aktif et
builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();