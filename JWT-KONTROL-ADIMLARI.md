# JWT ve Giriş Kontrol Adımları

## 1. Ortamı Hazırlama

- **Çalışan WebAPI’yi durdurun** (Cursor/terminalde çalışan `dotnet run` varsa Ctrl+C ile kapatın).
- Veritabanı güncel değilse:
  ```powershell
  cd c:\Users\HPY10519\Proje
  dotnet ef database update --project HRSupport.Infrastructure --startup-project HRSupport.WebAPI
  ```

## 2. Test Kullanıcısı (Admin)

- **E-posta:** `admin@hepiyi.com`  
- **Şifre:** `Admin123!`

Bu kullanıcı yoksa **bir kez** aşağıdaki SQL’i çalıştırın (SQL Server Management Studio veya `sqlcmd` ile):

- Script: `Scripts\SeedAdminUser.sql`  
- Veya doğrudan:
  ```sql
  IF NOT EXISTS (SELECT 1 FROM Employees WHERE Email = N'admin@hepiyi.com')
  BEGIN
      SET IDENTITY_INSERT Employees ON;
      INSERT INTO Employees (Id, FirstName, LastName, Email, Phone, CardID, BirthDate, StartDate, Department, Roles, PasswordHash, MustChangePassword, CreatedTime, Isactive, IsDeleted)
      VALUES (1, N'Sistem', N'Admin', N'admin@hepiyi.com', N'', 0, '1990-01-01', '2024-01-01', 1, 1, N'PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=', 0, '2024-01-01', 1, 0);
      SET IDENTITY_INSERT Employees OFF;
  END
  ```

## 3. Uygulamaları Başlatma

**Terminal 1 – WebAPI (port 5107):**
```powershell
cd c:\Users\HPY10519\Proje\HRSupport.WebAPI
dotnet run --launch-profile http
```

**Terminal 2 – UI (port 5062):**
```powershell
cd c:\Users\HPY10519\Proje\HRSupport.UI
dotnet run --launch-profile http
```

- UI adresi: **http://localhost:5062**
- API adresi: **http://localhost:5107** (UI `appsettings.json` içinde `ApiSettings:BaseUrl` bu adrese işaret etmeli).

## 4. Kontrol Listesi

| Adım | Ne yapacaksınız | Beklenen |
|------|------------------|----------|
| 1 | Tarayıcıda **http://localhost:5062** açın | Giriş sayfasına yönlendirilirsiniz (token yoksa). |
| 2 | **Yanlış** e-posta/şifre ile giriş yapın | "Giriş başarısız" benzeri hata. |
| 3 | **admin@hepiyi.com** / **Admin123!** ile giriş yapın | Anasayfaya (Dashboard) yönlendirilirsiniz; menüde Çalışanlar, Stajyerler, İzin Talepleri görünür (Admin). |
| 4 | Sağ üstte kullanıcı adı / çıkış görünüyor mu kontrol edin | Session’da Token, Role, FullName kayıtlı. |
| 5 | Çalışanlar veya İzin Talepleri sayfasına gidin | API’ye istek **Bearer token** ile gider; liste gelir (Admin tümünü görür). |
| 6 | Çıkış yapıp tekrar giriş yapın | Token temizlenir; tekrar login gerekir. |

## 5. API’yi Doğrudan Test (İsteğe Bağlı)

**Başarısız giriş (401):**
```powershell
Invoke-RestMethod -Uri "http://localhost:5107/api/Auth/login" -Method Post -Body '{"email":"test@test.com","password":"wrong"}' -ContentType "application/json"
# Beklenen: 401 veya hata mesajı
```

**Başarılı giriş (token döner):**
```powershell
$r = Invoke-RestMethod -Uri "http://localhost:5107/api/Auth/login" -Method Post -Body '{"email":"admin@hepiyi.com","password":"Admin123!"}' -ContentType "application/json"
$r.value.token   # JWT token
```

Sonra korumalı bir endpoint:
```powershell
$token = $r.value.token
Invoke-RestMethod -Uri "http://localhost:5107/api/Employee" -Headers @{ Authorization = "Bearer $token" }
# Beklenen: Çalışan listesi (JSON)
```

---

Özet: Önce API’yi durdurun, gerekirse `database update` ve `SeedAdminUser.sql` ile admin’i ekleyin; ardından WebAPI ve UI’ı başlatıp yukarıdaki adımlarla JWT ve girişi kontrol edin.
