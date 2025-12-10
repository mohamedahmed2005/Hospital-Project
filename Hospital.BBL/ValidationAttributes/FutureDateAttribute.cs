using System.ComponentModel.DataAnnotations;

namespace Hospital.BBL.ValidationAttributes
{
    /// <summary>
    /// Validates that a date is in the future (for appointments)
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        private readonly int _maxDaysInFuture;

        public FutureDateAttribute(int maxDaysInFuture = 365)
        {
            _maxDaysInFuture = maxDaysInFuture;
            ErrorMessage = $"Date must be in the future but not more than {maxDaysInFuture} days ahead";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle null
            }

            DateOnly date;

            if (value is DateOnly dateOnly)
            {
                date = dateOnly;
            }
            else if (value is DateTime dateTime)
            {
                date = DateOnly.FromDateTime(dateTime);
            }
            else if (DateOnly.TryParse(value.ToString(), out DateOnly parsedDate))
            {
                date = parsedDate;
            }
            else
            {
                return new ValidationResult("Invalid date format");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxDate = today.AddDays(_maxDaysInFuture);

            if (date <= today)
            {
                return new ValidationResult("Appointment date must be in the future");
            }

            if (date > maxDate)
            {
                return new ValidationResult($"Appointment date cannot be more than {_maxDaysInFuture} days in the future");
            }

            return ValidationResult.Success;
        }
    }
}
