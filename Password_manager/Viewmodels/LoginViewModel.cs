using Password_manager.Base;
using Password_manager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Password_manager.Viewmodels
{
    public class LoginViewModel : BaseModel
    {
        private string username;
        private string password;

        private ICommand loginCommand;
        private ICommand registerCommand;

        private WindowService windowService = new WindowService();
        
        public string Username {
            get => username;
            set 
            { 
                username = value; 
                OnPropertyChanged(); 
            } 
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                if(loginCommand == null)
                {
                    loginCommand = new RelayCommand(_ => Login(), _=>!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
                }
                return loginCommand;
            }
        }
        
        public ICommand RegisterCommand
        {
            get
            {
                if(registerCommand == null)
                {
                    registerCommand = new RelayCommand(param => OpenRegistration(param));
                }
                return registerCommand;
            }
        }

        private void Login()
        {

        }

        private void OpenRegistration(object param)
        {
            windowService.ShowWindow(new RegistrationView(), new RegistrationViewModel());
            var window = param as LoginView;
            windowService.CloseWindow(window);
        }
    }
}
