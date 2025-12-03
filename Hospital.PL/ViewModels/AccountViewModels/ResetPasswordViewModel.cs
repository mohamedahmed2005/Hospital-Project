using System.ComponentModel.DataAnnotations;

namespace Hospital.PL.ViewModels.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Password is required !")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirming Password is required !")]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
