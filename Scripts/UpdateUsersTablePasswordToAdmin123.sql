-- admin@hepiyi.com şifresini "Admin123!" yapar (Users tablosu).
-- Bu dosyayı SQL Server Management Studio veya sqlcmd ile çalıştırın.

UPDATE Users
SET PasswordHash = N'$2a$11$ewQZKQTkwGro6shiFyh2jer8YY5IlJ/JANX0X0GJrBxiSi0qpil2O'
WHERE Email = N'admin@hepiyi.com';

-- Kaç satır güncellendi (1 olmalı):
SELECT @@ROWCOUNT AS GuncellenenSatirSayisi;
