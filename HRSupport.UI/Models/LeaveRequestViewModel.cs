using System.ComponentModel.DataAnnotations;

namespace HRSupport.UI.Models
{
    public class LeaveRequestViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty; // API'den join ile gelmeli veya ID üzerinden gösterilmeli

        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        public DateTime EndDate { get; set; }

        [Display(Name = "İzin Türü")]
        public int Type { get; set; } // Enum ID: 1-Yıllık, 2-Hastalık vb.

        [Display(Name = "Durum")]
        public int Status { get; set; } // Enum ID: 1-Beklemede, 2-Red, 3-Onay

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;
    }

    public class CreateLeaveRequestViewModel
    {
        [Required(ErrorMessage = "Çalışan seçimi zorunludur")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi seçiniz")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Bitiş tarihi seçiniz")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

        public int Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }
    public class UpdateLeaveRequestViewModel
    {
        public int Id { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public int Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; } // Onay/Red durumunu değiştirebilmek için
    }
}