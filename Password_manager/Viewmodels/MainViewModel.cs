using Password_manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Password_manager.Base;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using System.Windows.Media;

namespace Password_manager.Viewmodels
{
    public class MainViewModel : BaseModel
    {
        private MasterAccount currentUser;
        private string masterPassword;
        private string newUsername;
        private string newPassword;
        private string newWebsite;
        private string statusMessage;
        private SolidColorBrush statusMessageColor;

        private ICommand addNewAccountCommand;
        private ICommand copyTextCommand;

        public ICommand CopyTextCommand
        {
            get
            {
                if(copyTextCommand == null)
                {
                    copyTextCommand = new RelayCommand(param => CopyText(param));
                }
                return copyTextCommand;
            }
        }

        public ICommand AddNewAccountCommand
        {
            get
            {
                if(addNewAccountCommand == null)
                {
                    addNewAccountCommand = new RelayCommand(_ => AddNewAccount(), _=> AllFieldsCompleted());
                }
                return addNewAccountCommand;
            }
        }

        public string NewUsername
        {
            get => newUsername;
            set
            {
                newUsername = value;
                OnPropertyChanged();
            }
        }

        public string NewPassword
        {
            get => newPassword;
            set
            {
                newPassword = value;
                OnPropertyChanged();
            }
        }

        public string NewWebsite
        {
            get => newWebsite;
            set
            {
                newWebsite = value;
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

        public MasterAccount CurrentUser
        {
            get => currentUser;
            set
            {
                currentUser = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel(MasterAccount user, string masterPassword)
        {
            CurrentUser = user;
            DecryptAllPasswords(masterPassword);
        }

        public MainViewModel()
        {
#if DEBUG
            DatabaseContext dbcontext = new DatabaseContext();
            CurrentUser = dbcontext.GetMasterAccountByUsername("step");
            masterPassword = "Numbthumb7!";
            DecryptAllPasswords(masterPassword);
#endif
        }

        public void AddNewAccount()
        {
            string generatedSalt = EncryptionService.GenerateSalt();
            string password = EncryptionService.Encrypt(NewPassword, masterPassword, generatedSalt);

            try
            {
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    Account newUser = new Account
                    {
                        Username = NewUsername,
                        EncryptedPassword = password,
                        AccountSalt = generatedSalt,
                        Website = NewWebsite,
                        MasterAccountId = CurrentUser.UserId
                    };
                    dbContext.Accounts.Add(newUser);
                    dbContext.SaveChanges();
                }
                ResetProperties();
                SetStatusMessage(true, "Your account has been added succesfully");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlException && sqlException.Number == 2601)
                {
                    foreach (SqlError error in sqlException.Errors)
                    {
                        if (error.Number == 2601)
                        {
                            if (error.Message.Contains("Username"))
                            {
                                
                            }
                            else if (error.Message.Contains("Email"))
                            {
                                
                            }
                            else
                            {
                                
                            }
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private bool AllFieldsCompleted()
        {
            return !string.IsNullOrEmpty(NewUsername) && !string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(NewWebsite);
        }

        public void CopyText(object param)
        {
            if(param != null)
            {
                string textToCopy = param as string;
                Clipboard.SetText(textToCopy);
            }


        }


        public void DecryptAllPasswords(string masterPassword)
        {
            foreach(Account acc in CurrentUser.Accounts)
            {
                acc.DecryptedPassword = EncryptionService.Decrypt(acc.EncryptedPassword, masterPassword, acc.AccountSalt);
            }
        }

        private void SetStatusMessage(bool successful, string message)
        {
            StatusMessageColor = successful ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            StatusMessage = message;
        }

        public void ResetProperties()
        {
            NewUsername = "";
            NewPassword = "";
            NewWebsite = "";
        }
    }
}
