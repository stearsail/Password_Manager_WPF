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
using MaterialDesignColors.Recommended;
using MaterialDesignThemes.Wpf;
using Password_manager.Views;
using Password_manager.Migrations;
using System.Collections.ObjectModel;

namespace Password_manager.Viewmodels
{
    public class MainViewModel : BaseModel
    {

        private string masterPassword;
        private string newUsername;
        private string newPassword;
        private string newSource;
        private string currentDialog;
        private string deleteDialogText;
        private string statusBarText = "";
        private SolidColorBrush statusBarColor;
        private PackIconKind statusBarIcon;
        private bool isDialogOpen;

        private MasterAccount currentUser;
        private ObservableCollection<Account> accounts;
        private Account currentAccount;

        private ICommand addAccountDialogCommand;
        private ICommand deleteAccountDialogCommand;
        private ICommand addNewAccountCommand;
        private ICommand editAccountCommand;
        private ICommand deleteAccountCommand;
        private ICommand copyTextCommand;
        private ICommand signOutCommand;

        private WindowService windowService = new WindowService();



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

        public ICommand EditAccountCommand
        {
            get
            {
                if(editAccountCommand == null)
                {
                    editAccountCommand = new RelayCommand(_ => EditAccount());
                }
                return editAccountCommand;
            }
        }
        public ICommand DeleteAccountCommand
        {
            get
            {
                if(deleteAccountCommand == null)
                {
                    deleteAccountCommand = new RelayCommand(_ => DeleteAccount());
                }
                return deleteAccountCommand;
            }
        }


        public ICommand SignOutCommand
        {
            get
            {
                if(signOutCommand == null)
                {
                    signOutCommand = new RelayCommand(param => SignOut(param));
                }
                return signOutCommand;
            }
        }

        public ICommand AddAccountDialogCommand
        {
            get
            {
                if(addAccountDialogCommand == null)
                {
                    addAccountDialogCommand = new RelayCommand(_ => AddAccountDialog());
                }
                return addAccountDialogCommand;
            }
        }

        public ICommand DeleteAccountDialogCommand
        {
            get
            {
                if(deleteAccountDialogCommand == null)
                {
                    deleteAccountDialogCommand = new RelayCommand(param => DeleteAccountDialog(param));
                }
                return deleteAccountDialogCommand;
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

        public string NewSource
        {
            get => newSource;
            set
            {
                newSource = value;
                OnPropertyChanged();
            }
        }


        public string CurrentDialog
        {
            get => currentDialog;
            set
            {
                currentDialog = value;
                OnPropertyChanged();
            }
        }

        public string DeleteDialogText
        {
            get => deleteDialogText;
            set
            {
                deleteDialogText = value;
                OnPropertyChanged();
            }
        }

        public string StatusBarText
        {
            get => statusBarText;
            set
            {
                statusBarText = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush StatusBarColor
        {
            get => statusBarColor;
            set
            {
                statusBarColor = value;
                OnPropertyChanged();
            }
        }

        public PackIconKind StatusBarIcon
        {
            get => statusBarIcon;
            set
            {
                statusBarIcon = value;
                OnPropertyChanged();
            }
        }

        public bool IsDialogOpen
        {
            get => isDialogOpen;
            set
            {
                isDialogOpen = value;
                OnPropertyChanged();
            }
        }

        public Account CurrentAccount
        {
            get => currentAccount;
            set
            {
                currentAccount = value;
                DeleteDialogText = $"Are you sure you want to delete \"{CurrentAccount.Username}\" from {CurrentAccount.Source}?";
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

        public string MasterPassword
        {
            get => masterPassword;
            set
            {
                masterPassword = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Account> Accounts
        {
            get => accounts;
            set
            {
                accounts = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(MasterAccount user, string masterPassword)
        {
            CurrentUser = user;
            MasterPassword = masterPassword;
            DecryptAllPasswords(MasterPassword);
        }

        public MainViewModel()
        {
//#if DEBUG
//            DatabaseContext dbcontext = new DatabaseContext();
//            CurrentUser = dbcontext.GetMasterAccountByUsername("step");
//            masterPassword = "Numbthumb7!";
//            DecryptAllPasswords(masterPassword);
//#endif
        }

        private void AddAccountDialog()
        {
            CurrentDialog = "AddAccountTemplate";
            IsDialogOpen = true;
        }

        private void AddNewAccount()
        {

            string username = NewUsername.ToLower();
            string generatedSalt = EncryptionService.GenerateSalt();
            string password = EncryptionService.Encrypt(NewPassword, MasterPassword, generatedSalt);

            try
            {
                using (DatabaseContext dbContext = new DatabaseContext())
                {
                    Account newUser = new Account
                    {
                        Username = username,
                        EncryptedPassword = password,
                        AccountSalt = generatedSalt,
                        Source = NewSource,
                        MasterAccountId = CurrentUser.UserId
                    };
                    dbContext.Accounts.Add(newUser);
                    dbContext.SaveChanges();

                    //Adding to ObservableCollection<Account> Accounts for UI to reflect changes
                    newUser.DecryptedPassword = EncryptionService.Decrypt(newUser.EncryptedPassword, MasterPassword, newUser.AccountSalt);
                    CurrentUser.Accounts.Add(newUser);
                    CurrentAccount = newUser;
                }
                ResetProperties();
                IsDialogOpen = false;
                SetStatusText(false, $"Succesfully added account \"{CurrentAccount.Username}\" from {CurrentAccount.Source}.");
            }
            catch (Exception ex)
            {
                SetStatusText(true, $"Error: {ex.Message}");
            }
        }


        public void EditAccount()
        {

        }

        public void DeleteAccount()
        {
            if( CurrentAccount != null)
            {
                using(var databaseContext = new DatabaseContext())
                {
                    databaseContext.Accounts.Remove(CurrentAccount);
                    databaseContext.SaveChanges();
                }
                CurrentUser.Accounts.Remove(CurrentAccount);
            }
            DialogHost.CloseDialogCommand.Execute(null, null);
            IsDialogOpen = false;
            SetStatusText(false, $"Succesfully deleted account \"{CurrentAccount.Username}\" from {CurrentAccount.Source}.");
        }

        private void DeleteAccountDialog(object param)
        {
            CurrentAccount = param as Account;
            CurrentDialog = "ConfirmDeleteTemplate";
            IsDialogOpen = true;
        }


        public void SignOut(object param)
        {
            windowService.ShowWindow(new LoginView(), new LoginViewModel());
            var window = param as MainView;
            windowService.CloseWindow(window);
        }

        private bool AllFieldsCompleted()
        {
            return !string.IsNullOrEmpty(NewUsername) && !string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(NewSource);
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

        public void SetStatusText(bool isError, string text)
        {
            StatusBarIcon = isError ? MaterialDesignThemes.Wpf.PackIconKind.ErrorOutline :
            MaterialDesignThemes.Wpf.PackIconKind.CheckOutline;
            StatusBarText = text;
            StatusBarColor = isError ? new SolidColorBrush(Colors.Orange) : new SolidColorBrush(Colors.Teal);
        }

        public void ResetProperties()
        {
            NewUsername = "";
            NewPassword = "";
            NewSource = "";
        }
    }
}
