using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TheBookClub.Models
{
    public class SecurePasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var reg = new Regex(@"^(?=.*?\d)(?=.*?[A-Z])(?=.*?[a-z])[A-Za-z\d,!@#$%^&*+=]{8,}$");
            if(value == null)
            {
                return new ValidationResult("Password must not be empty");
            }
            if (!reg.IsMatch((string)value))
                return new ValidationResult("Password must have at least 8 characters including: 1 number, 1 letter special character.");
            return ValidationResult.Success;
        }
    }
}