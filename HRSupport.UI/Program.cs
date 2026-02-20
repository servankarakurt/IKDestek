var builder = WebApplication.CreateBuilder(args);

// MVC Controller ve View'ları ekle
builder.Services.AddControllersWithViews();

// 1. API ile konuşmak için HttpClient'ı aktif et
builder.Services.AddHttpClient();

// 2. MVC tarafında kullanıcının giriş yaptığını hatırlamak için Cookie Authentication
builder.Services.AddAuthorization();

builder.Services.AddAuthentication("HRSupportCookie")
    .AddCookie("HRSupportCookie", options =>
    {
        options.LoginPath = "/Auth/Login"; // Giriş yapmamışsa buraya yönlendir
        options.Cookie.Name = "HRSupport.Auth";
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. Kimlik doğrulamayı aktif et
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();