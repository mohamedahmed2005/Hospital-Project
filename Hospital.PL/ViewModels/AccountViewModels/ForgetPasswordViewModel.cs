using System.ComponentModel.DataAnnotations;

namespace Hospital.PL.ViewModels.AccountViewModels
{
    public class ForgetPasswordViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is Required !")]
        public string Email { get; set; } = null!;
    }
}
