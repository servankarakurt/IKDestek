# Çalışan Listesi Boş / Ekleme Çalışmıyor – Kontrol Listesi

## Yapılan Kod Değişiklikleri (Özet)

- **Login:** JWT rolü `NormalizeRole()` ile "Admin", "IK", "Yönetici" olarak sabitlendi.
- **UI:** Token, controller içinde Session'dan alınıp her API isteğine ekleniyor (`CreateRequest`).
- **API:** Rol normalizasyonu (`NormalizeRoleClaimsTransformation`), camelCase JSON, `GetAll` için rol kontrolü.
- **Hata gösterimi:** Çalışanlar sayfasında tüm ModelState hataları kırmızı kutuda gösteriliyor.

## Sizin Kontrol Etmeniz Gerekenler

### 1. WebAPI çalışıyor mu?
- HRSupport.WebAPI projesini çalıştırın.
- Tarayıcıda `http://localhost:5107/swagger` veya `http://localhost:5107` açın; sayfa açılıyorsa API ayakta demektir.

### 2. UI’daki API adresi doğru mu?
- `HRSupport.UI/appsettings.json` içinde `ApiSettings:BaseUrl` değeri **WebAPI’nin dinlediği adres** olmalı.
- Varsayılan: `http://localhost:5107`
- WebAPI’yi farklı portta (ör. 5000) çalıştırıyorsanız bu değeri o adrese göre güncelleyin.

### 3. Yeni token alındı mı?
- **Çıkış yapıp** tekrar Admin ile giriş yapın. Eski oturumda rol eski formatta kalabilir.

### 4. Hata mesajını görmek
- Çalışanlar sayfasında **kırmızı “Hata” kutusu** çıkıyorsa, o kutudaki **tam metni** kopyalayıp paylaşın.
- Kutu yoksa tarayıcıda **`/Employee/Diagnostic`** sayfasına gidin (örn. `https://localhost:XXXX/Employee/Diagnostic`). Sayfadaki **tüm metni** kopyalayıp paylaşın.

### 5. Diagnostic çıktısı ne anlama gelir?
- **Role=(null)** → Oturumda rol yok; tekrar giriş yapın.
- **HasToken=False** → Token Session’da yok; tekrar giriş yapın veya cookie’leri kontrol edin.
- **ApiStatusCode=401** → Token geçersiz veya süresi dolmuş; çıkış yapıp tekrar giriş yapın.
- **ApiStatusCode=403** → Rol yetkisi yok; JWT’deki rol “Admin”/“IK”/“Yönetici” olmalı.
- **ApiStatusCode=0** veya **Exception: ...** → UI, API’ye bağlanamıyor; BaseUrl ve API’nin çalışıp çalışmadığını kontrol edin.
- **ApiStatusCode=200** ve **ResponsePreview** içinde `"value":[]` → API liste döndürüyor ama boş; veritabanında çalışan kaydı ve `IsDeleted=0` kontrol edin.

## Hâlâ Çalışmıyorsa

Lütfen şunlardan **birini** paylaşın:
1. Çalışanlar sayfasındaki **kırmızı hata kutusunun tam metni**, veya  
2. **/Employee/Diagnostic** sayfasının **tam çıktısı**.

Bu metin olmadan bir sonraki adımda doğrudan düzeltme öneremiyoruz.
