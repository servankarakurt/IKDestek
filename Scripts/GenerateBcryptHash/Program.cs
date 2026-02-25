using System;

// Şifre hash'i üretir (SQL script'lerinde PasswordHash alanı için).
// Kullanım: dotnet run [şifre]
// Örnek: dotnet run Personel123!
string password = args.Length > 0 ? args[0] : "Personel123!";
string hash = BCrypt.Net.BCrypt.HashPassword(password.Trim(), workFactor: 11);
Console.WriteLine(hash);
