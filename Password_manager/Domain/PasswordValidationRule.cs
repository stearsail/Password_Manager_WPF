using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Password_manager.Domain
{
    public class PasswordValidationRule : ValidationRule
    {
        bool isFirstLoad = true;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (isFirstLoad)
            {
                isFirstLoad = false;
                return ValidationResult.ValidResult;
            }
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Password is required.");
            }

            string password = value.ToString();

            bool isDigitPresent = password.Any(c => char.IsDigit(c));
            bool isSymbolPresent = password.Any(c => !char.IsLetterOrDigit(c));
            bool isUpperPresent = password.Any(c => char.IsUpper(c));

            if (!isDigitPresent)
            {
                return new ValidationResult(false, "Password must contain at least one digit");
            }

            if (!isSymbolPresent)
            {
                return new ValidationResult(false, "Password must contain at least one symbol");
            }
            if (!isUpperPresent)
            {
                return new ValidationResult(false, "Password must contain at least one capital letter");
            }

            return ValidationResult.ValidResult;
        }
    }
}
