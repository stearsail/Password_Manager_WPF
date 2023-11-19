using Password_manager.Base;
using Password_manager.Models;
using Password_manager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BCrypt;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using Microsoft.Data.SqlClient;
using System.Windows.Media;

namespace Password_manager.Viewmodels
{
    public class RegistrationViewModel : BaseModel
    {
        private string username;
        private string password;
        private string email;
        private string statusMessage;
        private SolidColorBrush statusMessageColor;

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

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(Password, salt);

            try
            {
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    MasterAccount newUser = new MasterAccount
                    {
                        Username = Username.ToLower(),
                        PasswordHash = passwordHash,
                        PasswordSalt = salt,
                        Email = Email,
                    };
                    dbContext.MasterAccounts.Add(newUser);
                    dbContext.SaveChanges();
                }

                ResetProperties();
                SetStatusMessage(true, "Your account has been succesfully registered.");

            }
            catch (DbUpdateException ex)
            {
                //trying to add duplicate key for Username or Email (these must be Unique values)
                if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlException && sqlException.Number == 2601)
                {
                    foreach (SqlError error in sqlException.Errors)
                    {
                        if (error.Number == 2601)
                        {
                            if (error.Message.Contains("Username"))
                            {
                                SetStatusMessage(false, $"The username: '{Username}'  is already in use.");
                                Username = "";
                            }
                            else if (error.Message.Contains("Email"))
                            {
                                SetStatusMessage(false, $"The email address: '{Email}' is already in use.");
                                Email = "";
                            }
                            else
                            {
                                SetStatusMessage(false, "A database error occurred during registration.");
                            }
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                SetStatusMessage(false, "An error occured during registration.");
            }
        }

        private void SetStatusMessage(bool successful, string message)
        {
            StatusMessageColor = successful ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            StatusMessage = message;
        }

        private void ResetProperties()
        {
            Username = "";
            Email = "";
            Password = "";
        }

        private void ReturnToLogin(object param)
        {
            windowService.ShowWindow(new LoginView(), new LoginViewModel());
            var window = param as RegistrationView;
            windowService.CloseWindow(window);
        }
    }
}
