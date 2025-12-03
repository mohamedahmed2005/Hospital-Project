using System.ComponentModel.DataAnnotations;

namespace Hospital.PL.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Please select a user type")]
        [Display(Name = "User Type")]
        public string UserType { get; set; } = null!; // "Doctor" or "Patient"

        public bool IsAgree { get; set; }
    }
}
