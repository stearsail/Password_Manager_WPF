using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Password_manager.Domain
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string val = value as string;
            if (string.IsNullOrEmpty(val))
                return new ValidationResult(false, "This field cannot be empty.");
            else
                return ValidationResult.ValidResult;
        }
    }
}
