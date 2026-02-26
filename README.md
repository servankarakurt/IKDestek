# HRSupport — İK Yönetim Uygulaması

Clean Architecture prensiplerine uygun, .NET 8 ile geliştirilmiş İnsan Kaynakları yönetim uygulaması.

## Özellikler

- **Kimlik doğrulama:** JWT ile giriş; Admin/İK (Users), Çalışan/Yönetici (Employees), Stajyer (Interns) hesapları.
- **Çalışan ve stajyer yönetimi:** CRUD, notlar, mentor atama, stajyer görevleri.
- **İzin talepleri:** Personel izin talebi açar; İK/Admin/Yönetici onaylar veya reddeder; yıllık izin bakiyesi otomatik güncellenir.
- **Dashboard:** Bekleyen/onaylanan izin sayıları, departman dağılımı, son talepler listesi (onayla/reddet/revizyon).
- **Personel paneli:** Çalışan/Stajyer kendi izin bakiyesi, talepleri ve (stajyer için) mentor bilgisini görür.

## Gereksinimler

- .NET 8 SDK
- SQL Server (LocalDB veya tam sürüm)
- İsteğe bağlı: Visual Studio 2022 / VS Code / Rider

## Çözüm Yapısı

| Proje | Açıklama |
|-------|----------|
| **HRSupport.Domain** | Entity'ler, enum'lar; dış bağımlılık yok |
| **HRSupport.Application** | Use case'ler (MediatR), DTO, repository arayüzleri, validasyon |
| **HRSupport.Infrastructure** | EF Core, repository implementasyonları, DbContext, migration'lar |
| **HRSupport.WebAPI** | REST API, JWT kimlik doğrulama, Swagger |
| **HRSupport.UI** | ASP.NET Core MVC; API'ye Session'daki JWT ile istek atar |
| **HRSupport.Tests** | Unit testler (xUnit) |

## Kurulum ve Çalıştırma

### 1. Veritabanı

- SQL Server'da yeni bir veritabanı oluşturun (örn. `HRSupport` veya projede kullanılan `HRSupportDB_2026`).
- `HRSupport.WebAPI` içindeki `appsettings.json` (veya `appsettings.Development.json`) dosyasında bağlantı dizesini ayarlayın:

```json
{
  "ConnectionStrings": {
    "SqlConnection": "Server=(localdb)\\mssqllocaldb;Database=HRSupport;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

- Migration'ları uygulayın:

```bash
cd HRSupport.Infrastructure
dotnet ef database update --startup-project ../HRSupport.WebAPI
```

- İlk kullanıcılar için `Scripts` klasöründeki SQL script'lerini çalıştırın (örn. `SeedAdminUser.sql`, `SeedPersonelUser.sql`). Şifre hash üretmek için `Scripts/GenerateBcryptHash` veya `Scripts/GeneratePasswordHash` araçlarını kullanabilirsiniz.

### 2. WebAPI

```bash
cd HRSupport.WebAPI
dotnet run
```

Varsayılan olarak API `http://localhost:5107` (veya `launchSettings.json` içindeki port) üzerinde çalışır. Swagger: `http://localhost:5107` veya `http://localhost:5107/swagger`.

### 3. UI (MVC)

```bash
cd HRSupport.UI
dotnet run
```

Tarayıcıda uygulama adresi açılır (örn. `https://localhost:7xxx`). Giriş için Scripts ile oluşturulmuş Admin/İK veya personel hesabı kullanın.

### 4. API Adresi (UI → API)

UI'ın API'ye istek atabilmesi için `HRSupport.UI/appsettings.json` (veya `appsettings.Development.json`) içinde WebAPI adresi tanımlı olmalıdır:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5107"
  }
}
```

Portlarınız farklıysa bu adresi güncelleyin. WebAPI tarafında `JwtSettings:Audience` listesinde UI'ın çalıştığı origin (örn. `https://localhost:7127`) tanımlı olmalıdır; aksi halde token doğrulama hatası alabilirsiniz.

## Roller ve Erişim

| Rol | Yetkiler |
|-----|----------|
| **Admin / İK** | Çalışan ve stajyer CRUD, izin onayı/reddi/revizyon, dashboard, notlar, şifre sıfırlama. |
| **Yönetici** | Kendi birimindeki çalışan sayısı, biriminden bekleyen izinler, onay/red/revizyon. |
| **Çalışan / Stajyer** | Kendi paneli (PersonelPanel), izin talebi oluşturma, kalan izin, talepler listesi, (stajyer için) mentor bilgisi. |

## Test

```bash
cd HRSupport.Tests
dotnet test
```

İzin tarih validasyonu, çalışan validasyonu, not ekleme vb. use case'ler unit test ile kapsanmaktadır.

## Dokümantasyon

- **Mimari:** [MIMARI.md](MIMARI.md) — Katmanlar, bağımlılıklar, örnek akış (izin talebi).
- **Veri modeli:** [VERI-MODELI.md](VERI-MODELI.md) — Tablolar ve ilişkiler (ER özeti).

## Lisans

Bu proje eğitim/şirket içi kullanım için hazırlanmıştır.
