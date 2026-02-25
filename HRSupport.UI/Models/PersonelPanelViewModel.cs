namespace HRSupport.UI.Models
{
    /// <summary>Stajyer paneli: kalan gün, son 5 talep, tüm talepler, mentor bilgisi.</summary>
    public class StajyerPanelViewModel
    {
        public int KalanIzinGunu { get; set; }
        public List<LeaveRequestViewModel> SonBesTalep { get; set; } = new();
        public List<LeaveRequestViewModel> Taleplerim { get; set; } = new();
        public MentorInfoViewModel? Mentor { get; set; }
    }

    /// <summary>Çalışan paneli: izin bilgileri + birim arkadaşları + mentee listesi.</summary>
    public class CalisanPanelViewModel
    {
        public int KalanIzinGunu { get; set; }
        public List<LeaveRequestViewModel> SonBesTalep { get; set; } = new();
        public List<LeaveRequestViewModel> Taleplerim { get; set; } = new();
        public List<ColleagueViewModel> BirimArkadaslari { get; set; } = new();
        public List<MenteeViewModel> MenteeListesi { get; set; } = new();
    }

    public class MentorInfoViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class ColleagueViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class MenteeViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
