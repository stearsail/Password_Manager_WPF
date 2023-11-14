using Microsoft.EntityFrameworkCore;
using Password_manager.Base;
using Password_manager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Password_manager.Viewmodels
{
    public class LoginViewModel : BaseModel
    {
        private string username;
        private string password;
        private string statusMessage;
        private SolidColorBrush statusMessageColor;
        private bool isLogging;

        private ICommand loginCommand;
        private ICommand registerCommand;

        private readonly DatabaseContext databaseContext = new DatabaseContext();
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

        public string StatusMessage
        {
            get => statusMessage;
            set
            {
                statusMessage = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush StatusMessageColor
        {
            get => statusMessageColor;
            set
            {
                statusMessageColor = value;
                OnPropertyChanged();
            }
        }

        public bool IsLogging
        {
            get => isLogging;
            set
            {
                isLogging = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                if(loginCommand == null)
                {
                    loginCommand = new RelayCommand(async param => await Login(param), _=>!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
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

        private async Task Login(object param)
        {
            IsLogging = true;

            await Task.Run(async () =>
            {
                if (!await VerifyUsername(Username))
                {
                    SetStatusMessage(false, "The username does not exist.");
                }
                else
                {
                    var username = await databaseContext.MasterAccounts.FirstOrDefaultAsync(u => u.Username == Username);
                    string passwordHash = (username?.PasswordHash).ToString();
                    string passwordSalt = (username?.PasswordSalt).ToString();
                    if (!VerifyPassword(Password, passwordHash, passwordSalt))
                    {
                        SetStatusMessage(false, "Password is incorrect.");
                    }
                    else
                    {
                        OpenMainWindow(param);
                    }
                }
            });

            IsLogging = false;
        }

        private async Task<bool> VerifyUsername(string username)
        {
            return await databaseContext.MasterAccounts.AnyAsync(u => u.Username == username);
        }

        private bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            string hashedEnteredPassword = BCrypt.Net.BCrypt.HashPassword(enteredPassword, storedSalt);
            return hashedEnteredPassword == storedHash;
        }
        
        private void SetStatusMessage(bool successful, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusMessageColor = successful ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                StatusMessage = message;
            });
        }

        private void OpenRegistration(object param)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                windowService.ShowWindow(new RegistrationView(), new RegistrationViewModel());
                var window = param as LoginView;
                windowService.CloseWindow(window);
            });
        }

        private void OpenMainWindow(object param)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                windowService.ShowWindow(new MainView(), new MainViewModel());
                var window = param as LoginView;
                windowService.CloseWindow(window);
            });
        }
    }
}
