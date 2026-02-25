using System.ComponentModel.DataAnnotations;

namespace HRSupport.UI.Models
{
    public class InternViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        [Display(Name = "Üniversite")]
        public string? University { get; set; }
        [Display(Name = "Bölüm")]
        public string? Major { get; set; }
        [Display(Name = "Sınıf")]
        public int Grade { get; set; }
        [Display(Name = "Başlangıç")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Bitiş")]
        public DateTime EndDate { get; set; }
        public string? MentorName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }

    public class CreateInternViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Soyad zorunludur")]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? University { get; set; }
        public string? Major { get; set; }
        public int Grade { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(3);
        public int? MentorId { get; set; } // Bir Employee ID'si
    }
    public class UpdateInternViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad zorunludur")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Soyad zorunludur")]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? University { get; set; }
        public string? Major { get; set; }
        public int Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MentorId { get; set; }
    }

    public class InternDetailViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? University { get; set; }
        public string? Major { get; set; }
        public int Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? MentorName { get; set; }
        public List<InternTaskItemViewModel> Tasks { get; set; } = new();
        public List<MentorNoteItemViewModel> MentorNotes { get; set; } = new();
    }

    public class InternTaskItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class MentorNoteItemViewModel
    {
        public int Id { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public DateTime? NoteDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? MentorName { get; set; }
    }
}