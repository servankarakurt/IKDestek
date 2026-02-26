using System;

namespace HRSupport.Application.DTOs
{
    /// <summary>İzin formu yazdırma sayfası için API yanıtı. Personel kendi iznini yazdırabilir (yetki izin sahipliğine göre).</summary>
    public class LeaveRequestPrintInfoDto
    {
        public DateTime FormPrintDate { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Kurum { get; set; } = "Hepiyi Sigorta";
        public string Sirket { get; set; } = "Hepiyi Sigorta A.Ş.";
        public string DepartmentName { get; set; } = string.Empty;
        public string Unvan { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public int LeaveYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RequestedDays { get; set; }
        public string AddressAndPhone { get; set; } = string.Empty;
        public DateTime EmployeeStartDate { get; set; }
        public string KullanilanIzinGunuDisplay { get; set; } = string.Empty;
    }
}
