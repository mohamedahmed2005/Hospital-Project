using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hospital.BBL.ValidationAttributes
{
    /// <summary>
    /// Validates Egyptian phone numbers (11 digits starting with 01)
    /// </summary>
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        private const string PhonePattern = @"^01[0-2,5]{1}[0-9]{8}$";

        public PhoneNumberValidationAttribute()
        {
            ErrorMessage = "Phone number must be 11 digits and start with 01 (e.g., 01012345678)";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let [Required] handle null/empty
            }

            string phoneNumber = value.ToString()!.Trim();

            // Remove any spaces, dashes, or parentheses
            phoneNumber = Regex.Replace(phoneNumber, @"[\s\-\(\)]", "");

            // Check if it matches the Egyptian phone pattern
            if (!Regex.IsMatch(phoneNumber, PhonePattern))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true; // Let [Required] handle null/empty
            }

            string phoneNumber = value.ToString()!.Trim();
            phoneNumber = Regex.Replace(phoneNumber, @"[\s\-\(\)]", "");

            return Regex.IsMatch(phoneNumber, PhonePattern);
        }
    }
}
