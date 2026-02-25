using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace HRSupport.Application.Common
{
    /// <summary>
    /// Veritabanındaki hem BCrypt (migration seed) hem SHA256 (script) hash'leriyle uyumlu doğrulama.
    /// Yeni şifreler BCrypt ile hash'lenir.
    /// </summary>
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            return BCrypt.Net.BCrypt.HashPassword(password.Trim(), workFactor: 11);
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password)) return false;
            var hash = storedHash?.Trim();
            if (string.IsNullOrEmpty(hash)) return false;
            var pwd = password.Trim();

            // Migration ile gelen veya BCrypt ile kaydedilmiş hash'ler
            if (hash.StartsWith("$2a$", StringComparison.Ordinal) ||
                hash.StartsWith("$2b$", StringComparison.Ordinal) ||
                hash.StartsWith("$2y$", StringComparison.Ordinal))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(pwd, hash);
                }
                catch
                {
                    return false;
                }
            }

            // Eski script / SHA256 + Base64 hash'ler (örn. SeedAdminUser.sql)
            var computed = HashSha256(pwd);
            return computed.Equals(hash, StringComparison.Ordinal);
        }

        private static string HashSha256(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
