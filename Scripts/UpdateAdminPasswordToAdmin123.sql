-- Bu script admin@hepiyi.com kullanıcısının şifresini "Admin123!" yapar.
-- Giriş hep "yanlış şifre" diyorsa, bu script'i çalıştırıp tekrar deneyin.
-- SHA256 + Base64 hash (Admin123!)

UPDATE Employees
SET PasswordHash = N'PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE='
WHERE Email = N'admin@hepiyi.com';

-- Kaç satır güncellendi kontrol (0 = bu e-posta ile kayıt yok)
SELECT @@ROWCOUNT AS GuncellenenSatirSayisi;
