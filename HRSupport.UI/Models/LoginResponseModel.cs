namespace HRSupport.UI.Models
{
    // API'den Result.Value içinden dönen asıl veri
    public class LoginResponseModel
    {
        public string Token { get; set; } // Hata veren yer burasıydı, eklendi!
        public string Email { get; set; }
    }
}