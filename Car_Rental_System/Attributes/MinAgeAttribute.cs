using System;
using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.Attributes // Đổi 'YourNamespace' thành namespace của bạn
{
    public class MinAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        public MinAgeAttribute(int minAge)
        {
            _minAge = minAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                if (dateOfBirth.AddYears(_minAge) > DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? $"Bạn phải từ {_minAge} tuổi trở lên.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
