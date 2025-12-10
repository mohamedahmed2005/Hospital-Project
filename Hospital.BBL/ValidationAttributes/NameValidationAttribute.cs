using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hospital.BBL.ValidationAttributes
{
    /// <summary>
    /// Validates that a name contains only letters and spaces (no numbers or special characters)
    /// </summary>
    public class NameValidationAttribute : ValidationAttribute
    {
        private const string NamePattern = @"^[a-zA-Z\u0600-\u06FF\s]+$"; // Supports English and Arabic letters

        public NameValidationAttribute()
        {
            ErrorMessage = "Name can only contain letters and spaces (no numbers or special characters)";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Let [Required] handle null/empty
            }

            string name = value.ToString()!.Trim();

            // Check minimum length
            if (name.Length < 2)
            {
                return new ValidationResult("Name must be at least 2 characters long");
            }

            // Check if it matches the name pattern
            if (!Regex.IsMatch(name, NamePattern))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true;
            }

            string name = value.ToString()!.Trim();

            if (name.Length < 2)
            {
                return false;
            }

            return Regex.IsMatch(name, NamePattern);
        }
    }
}
