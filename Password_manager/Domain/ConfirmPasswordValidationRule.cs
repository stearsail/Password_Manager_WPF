using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Password_manager.Domain
{
    public class ConfirmPasswordValidationRule : ValidationRule
    {

        public Wrapper MyWrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string val = value as string;
            if (val != (string)MyWrapper.Password)
                return new ValidationResult(false, "Passwords must match!");
            else
                return ValidationResult.ValidResult;
        }
    }

    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy),
            new PropertyMetadata(null));
    }

    public class Wrapper : DependencyObject
    {
        public static readonly DependencyProperty PasswordProperty =
             DependencyProperty.Register("Password", typeof(object), typeof(Wrapper));

        public object Password
        {
            get { return GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
    }

}
