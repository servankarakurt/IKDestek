# CRUD / API Cevap Dönmeme – Analiz ve Plan

## Yapılan Genel Analiz

- **UI (HRSupport.UI)** MVC ile sayfa sunuyor; tüm CRUD işlemleri **WebAPI (HRSupport.WebAPI)** üzerinden yapılıyor.
- UI, `ApiSettings:BaseUrl` ile API adresini alıyor; her istekte Session’daki JWT’yi `Authorization: Bearer ...` ile gönderiyor.
- API, JWT ve `[Authorize(Roles = "Admin,IK")]` / `[Authorize(Roles = "Admin,IK,Yönetici")]` ile yetkilendirme yapıyor.

## Tespit Edilen Olası Nedenler ve Yapılan Düzeltmeler

### 1. HTTPS yönlendirme (API cevap vermiyor / bağlantı hatası)

**Sorun:** WebAPI’de `UseHttpsRedirection()` açıktı. UI `http://localhost:5107` ile istek atınca API 307 ile `https://localhost:7153` adresine yönlendiriyordu. HttpClient yönlendirmeyi takip ederken geliştirme sertifikası veya ağ ayarları yüzünden istek düşebilir veya “cevap gelmiyor” hissi oluşabilir.

**Yapılan:** Development ortamında HTTPS yönlendirme kapatıldı. Böylece `http://localhost:5107` ile gelen istekler doğrudan HTTP üzerinden cevaplanıyor.

- **Dosya:** `HRSupport.WebAPI/Program.cs`

### 2. HttpClient timeout ve adres

**Sorun:** Timeout tanımlı değildi; API yavaş veya takılı kalırsa UI uzun süre bekleyebilir veya “cevap yok” gibi görünebilirdi.

**Yapılan:**  
- Varsayılan HttpClient için 30 saniye timeout eklendi.  
- `ApiWithAuth` named client’a BaseAddress ve 30 saniye timeout verildi.

- **Dosya:** `HRSupport.UI/Program.cs`

### 3. ApiResult null güvenliği

**Sorun:** API’den gelen JSON’da `error` veya `errors` bazen null/eksik olabiliyor; deserialize sonrası kullanımda NullReferenceException riski vardı.

**Yapılan:** `ApiResult<T>` içinde `Value`, `Error`, `Errors` nullable yapıldı.

- **Dosya:** `HRSupport.UI/Models/ApiResult.cs`

### 4. API adresi (Development)

**Sorun:** Development’ta API adresi sadece ana `appsettings.json`’da vardı; ortama özel override yoktu.

**Yapılan:** `appsettings.Development.json` içine `ApiSettings:BaseUrl: http://localhost:5107` eklendi (isteğe bağlı override için).

- **Dosya:** `HRSupport.UI/appsettings.Development.json`

---

## Sizin Kontrol Etmeniz Gerekenler

1. **WebAPI gerçekten çalışıyor mu?**  
   - HRSupport.WebAPI projesini çalıştırın.  
   - Tarayıcıda `http://localhost:5107` veya `http://localhost:5107/swagger` açın; sayfa geliyorsa API ayakta demektir.

2. **Port eşleşiyor mu?**  
   - WebAPI’nin `Properties/launchSettings.json` içindeki `applicationUrl` (ör. `http://localhost:5107`) ile UI’daki `ApiSettings:BaseUrl` aynı olmalı.  
   - Farklı port kullanıyorsanız UI’da `appsettings.json` veya `appsettings.Development.json` içinde BaseUrl’i güncelleyin.

3. **Giriş ve token**  
   - CRUD sayfalarına Admin veya IK ile giriş yapın.  
   - Eski oturumda token/rol uyumsuzluğu olabilir; **çıkış yapıp tekrar giriş** deneyin.

4. **Hata mesajını görmek**  
   - Çalışanlar için: `/Employee/Diagnostic` sayfasına gidin (giriş yaptıktan sonra).  
   - Sayfada BaseUrl, token varlığı, API’den dönen status kodu ve yanıt önizlemesi yazar.  
   - **ApiStatusCode=0** veya **Exception:** → UI API’ye ulaşamıyor (adres/port/API kapalı).  
   - **401** → Token yok/geçersiz/süresi dolmuş; tekrar giriş yapın.  
   - **403** → Rol yetkisi yok; JWT’deki rol Admin/IK/Yönetici olmalı.  
   - **200** ama boş liste → Yetki tamam, veritabanında kayıt yok veya filtre nedeniyle boş.

---

## Özet Kontrol Listesi

| Kontrol | Açıklama |
|--------|-----------|
| WebAPI çalışıyor | `http://localhost:5107` veya `/swagger` açılıyor |
| BaseUrl doğru | UI `ApiSettings:BaseUrl` = WebAPI’nin dinlediği adres |
| Admin/IK ile giriş | CRUD için gerekli roller |
| Çıkış / tekrar giriş | Eski token/rol sorunlarını elemek için |
| Diagnostic sayfası | `/Employee/Diagnostic` ile gerçek hata/yanıtı görün |

Bu düzeltmeler ve kontrollerle CRUD’da “API cevap döndermiyor / kayıt yapılamıyor” sorunlarının büyük kısmı giderilmiş olmalı. Hâlâ devam ederse, Diagnostic çıktısını veya sayfadaki hata kutusunun tam metnini paylaşmanız yeterli olur.
