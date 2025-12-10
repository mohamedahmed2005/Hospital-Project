using System.ComponentModel.DataAnnotations;

namespace Hospital.BBL.ValidationAttributes
{
    /// <summary>
    /// Validates that a date is in the past (for date of birth)
    /// </summary>
    public class PastDateAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;
        private readonly int _maximumAge;

        public PastDateAttribute(int minimumAge = 0, int maximumAge = 150)
        {
            _minimumAge = minimumAge;
            _maximumAge = maximumAge;
            ErrorMessage = $"Date must be between {minimumAge} and {maximumAge} years ago";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle null
            }

            DateTime date;

            if (value is DateTime dateTime)
            {
                date = dateTime;
            }
            else if (DateTime.TryParse(value.ToString(), out DateTime parsedDate))
            {
                date = parsedDate;
            }
            else
            {
                return new ValidationResult("Invalid date format");
            }

            var today = DateTime.Today;
            var minDate = today.AddYears(-_maximumAge);
            var maxDate = today.AddYears(-_minimumAge);

            if (date > maxDate)
            {
                return new ValidationResult($"Date must be at least {_minimumAge} year(s) ago");
            }

            if (date < minDate)
            {
                return new ValidationResult($"Date cannot be more than {_maximumAge} years ago");
            }

            return ValidationResult.Success;
        }
    }
}
