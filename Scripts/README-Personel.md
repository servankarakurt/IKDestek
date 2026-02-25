# Personel ekleme ve giriş

## 1. DB'ye test personel ekleme (şifre ile)

**Script:** `SeedPersonelUser.sql`

- **E-posta:** personel@hepiyi.com  
- **Şifre:** Personel123!  
- **Rol:** Çalışan  

SSMS veya sqlcmd ile bu script'i çalıştırın. Ardından bu hesapla giriş yapabilirsiniz; yönlendirme **Panelim** (PersonelPanel) sayfasına yapılır.

## 2. Kendi şifreniz için hash üretme

Farklı bir şifre ile personel eklemek için:

```bash
cd Scripts/GenerateBcryptHash
dotnet run -- YourSifreniz
```

Çıkan hash'i kopyalayıp SQL'deki `PasswordHash` alanına yapıştırın.

## 3. Personel girişi

Login sırası: **Users** → **Employees** → **Interns**.  
personel@hepiyi.com önce Employees tablosunda aranır; şifre `PasswordHelper.Verify` ile doğrulanır. Başarılı girişte rol **Çalışan** olur ve **PersonelPanel** açılır.
