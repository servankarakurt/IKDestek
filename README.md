# HRSupport — İK Yönetim Uygulaması

Clean Architecture prensiplerine uygun, .NET 8 ile geliştirilmiş İnsan Kaynakları yönetim uygulaması.

## Gereksinimler

- .NET 8 SDK
- SQL Server (LocalDB veya tam sürüm)
- İsteğe bağlı: Visual Studio 2022 / VS Code / Rider

## Çözüm Yapısı

| Proje | Açıklama |
|-------|----------|
| **HRSupport.Domain** | Entity'ler, enum'lar; dış bağımlılık yok |
| **HRSupport.Application** | Use case'ler (MediatR), DTO, repository arayüzleri, validasyon |
| **HRSupport.Infrastructure** | EF Core + Dapper, repository implementasyonları, DbContext |
| **HRSupport.WebAPI** | REST API, JWT kimlik doğrulama |
| **HRSupport.UI** | ASP.NET Core MVC, giriş ve CRUD arayüzü |
| **HRSupport.Tests** | Unit testler (xUnit) |

## Kurulum ve Çalıştırma

### 1. Veritabanı

- SQL Server'da yeni bir veritabanı oluşturun (örn. `HRSupport`).
- `HRSupport.WebAPI` içindeki `appsettings.json` (veya `appsettings.Development.json`) dosyasında bağlantı dizesini ayarlayın:

```json
{
  "ConnectionStrings": {
    "SqlConnection": "Server=(localdb)\\mssqllocaldb;Database=HRSupport;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

- Paketleri geri yükleyip migration'ları uygulayın:

```bash
cd HRSupport.Infrastructure
dotnet ef database update --startup-project ../HRSupport.WebAPI
```

- İlk kullanıcılar ve test verileri için `Scripts` klasöründeki SQL script'lerini (örn. seed) çalıştırabilirsiniz.

### 2. WebAPI

```bash
cd HRSupport.WebAPI
dotnet run
```

Varsayılan olarak API `http://localhost:5107` (veya launchSettings'te tanımlı port) üzerinde çalışır. Swagger: `http://localhost:5107` veya `http://localhost:5107/swagger`.

### 3. UI (MVC)

```bash
cd HRSupport.UI
dotnet run
```

Tarayıcıda uygulama adresi açılır (örn. `https://localhost:7xxx`). Giriş için Admin/İK kullanıcısı gerekir (Scripts ile oluşturulmuş olmalı).

### 4. API Adresi (UI → API)

UI'ın API'ye istek atabilmesi için `HRSupport.UI/appsettings.json` veya `appsettings.Development.json` içinde WebAPI adresi tanımlı olmalıdır:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5107"
  }
}
```

Portlarınız farklıysa bu adresi güncelleyin.

## Roller ve Erişim

- **Admin / İK:** Çalışan ve stajyer CRUD, izin onayı, dashboard, notlar.
- **Yönetici:** Kendi birimindeki çalışan sayısı, biriminden bekleyen izinler, onay/red.
- **Çalışan / Stajyer:** Kendi paneli, izin talebi oluşturma, kalan izin, talepler listesi.

## Test

```bash
cd HRSupport.Tests
dotnet test
```

En az 5 use case (izin tarih validasyonu, çalışan validasyonu, not ekleme vb.) unit test ile kapsanmaktadır.

## Dokümantasyon

- **Mimari:** [MIMARI.md](MIMARI.md) — Katmanlar, bağımlılıklar, örnek akış.
- **Veri modeli:** [VERI-MODELI.md](VERI-MODELI.md) — Tablolar ve ilişkiler (ER özeti).

## Lisans

Bu proje eğitim/şirket içi kullanım için hazırlanmıştır.
