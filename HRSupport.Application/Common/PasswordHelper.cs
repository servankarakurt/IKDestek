using System.Security.Cryptography;
using System.Text;

namespace HRSupport.Application.Common
{
    /// <summary>
    /// Mevcut veritabanındaki hash'lerle uyumlu SHA256 tabanlı hash/doğrulama.
    /// </summary>
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password)) return false;
            var hash = storedHash?.Trim();
            if (string.IsNullOrEmpty(hash)) return false;
            var computed = Hash(password.Trim());
            return computed.Equals(hash, StringComparison.Ordinal);
        }
    }
}
