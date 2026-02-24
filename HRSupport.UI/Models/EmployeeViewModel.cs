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
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public int Department { get; set; }
        public int Roles { get; set; }
    }
}