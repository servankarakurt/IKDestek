using System.ComponentModel.DataAnnotations;

namespace HRSupport.UI.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = string.Empty;
        [Display(Name = "Departman")]
        public int Department { get; set; }
        [Display(Name = "Rol")]
        public int Roles { get; set; }
        [Display(Name = "İşe Başlama")]
        public DateTime StartDate { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }

    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Soyad zorunludur")]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        [Display(Name = "TC Kimlik No (opsiyonel)")]
        public string? TCKN { get; set; }
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.Now.AddYears(-20);
        public DateTime StartDate { get; set; } = DateTime.Now;
        public int Department { get; set; }
        public int Roles { get; set; }
    }
    public class UpdateEmployeeViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad zorunludur")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Soyad zorunludur")]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        [Display(Name = "TC Kimlik No (opsiyonel)")]
        public string? TCKN { get; set; }
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public int Department { get; set; }
        public int Roles { get; set; }
    }

    public class EmployeeDetailViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? TCKN { get; set; }
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string RolesName { get; set; } = string.Empty;
        public List<EmployeeNoteItemViewModel> Notes { get; set; } = new();
        public List<LeaveRequestItemViewModel> LeaveHistory { get; set; } = new();
    }

    public class EmployeeNoteItemViewModel
    {
        public int Id { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class LeaveRequestItemViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}