using Password_manager.Base;
using Password_manager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Password_manager.Viewmodels
{
    public class RegistrationViewModel : BaseModel
    {
        private string username;
        private string password;
        private string email;

        private ICommand registerCommand;
        private ICommand returnToLoginCommand;

        private WindowService windowService = new WindowService();


        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get=> password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null)
                {
                    registerCommand = new RelayCommand(_ => Register(), _ => AllFieldsCompleted());
                }
                return registerCommand;
            }
        }

        public ICommand ReturnToLoginCommand
        {
            get
            {
                if(returnToLoginCommand == null)
                {
                    returnToLoginCommand = new RelayCommand(param => ReturnToLogin(param));
                }
                return returnToLoginCommand;
            }
        }

        private bool AllFieldsCompleted()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email);
        }

        private void Register()
        {

        }

        private void ReturnToLogin(object param)
        {
            windowService.ShowWindow(new LoginView(), new LoginViewModel());
            var window = param as RegistrationView;
            windowService.CloseWindow(window);
        }
    }
}
