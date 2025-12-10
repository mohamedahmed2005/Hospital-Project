using Hospital.BBL.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Hospital.PL.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [NameValidation]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [NameValidation]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [PhoneNumberValidation]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Department { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsUrgent { get; set; }
    }
}


