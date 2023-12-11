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
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Data;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace Password_manager.Viewmodels
{
    public class MainViewModel : BaseModel
    {

        private string masterPassword;
        private string newUsername;
        private string newPassword;
        private string newSource;
        private string applicationPath;
        private BitmapImage newBitmapIcon;
        private byte[] newIcon;
        private Models.SourceType selectedSourceType;
        private string currentDialog;
        private string deleteDialogText;
        private string statusBarText = "";
        private string searchText;
        private SolidColorBrush statusBarColor;
        private PackIconKind statusBarIcon;
        private bool isDialogOpen;

        private MasterAccount currentUser;
        private ObservableCollection<AccountGroupInfo> groupedAccounts;
        private Account currentAccount;

        private ICommand addAccountDialogCommand;
        private ICommand deleteAccountDialogCommand;
        private ICommand addNewAccountCommand;
        private ICommand editAccountDialogCommand;
        private ICommand closeEditAccountDialogCommand;
        private ICommand editAccountCommand;
        private ICommand deleteAccountCommand;
        private ICommand copyTextCommand;
        private ICommand signOutCommand;
        private ICommand openLinkCommand;
        private ICommand selectApplicationCommand;

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
                    editAccountCommand = new RelayCommand(_ => EditAccount(), _=> AllEditFieldsCompleted());
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

        public ICommand EditAccountDialogCommand
        {
            get
            {
                if(editAccountDialogCommand == null)
                {
                    editAccountDialogCommand = new RelayCommand(param => EditAccountDialog(param));
                }
                return editAccountDialogCommand;
            }
        }

        public ICommand CloseEditAccountDialogCommand
        {
            get
            {
                if(closeEditAccountDialogCommand == null)
                {
                    closeEditAccountDialogCommand = new RelayCommand(param => CloseEditAccountDialog());
                }
                return closeEditAccountDialogCommand;
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
        public ICommand OpenLinkCommand
        {
            get
            {
                if(openLinkCommand == null)
                {
                   openLinkCommand = new RelayCommand(param => OpenLink(param));
                }
                return openLinkCommand;
            }
        }

        public ICommand SelectApplicationCommand
        {
            get
            {
                if(selectApplicationCommand == null)
                {
                    selectApplicationCommand = new RelayCommand(_ => SelectApplication());
                }
                return selectApplicationCommand;
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

        public string ApplicationPath
        {
            get => applicationPath;
            set
            {
                applicationPath = value;
                OnPropertyChanged();
            }
        }


        public BitmapImage NewBitmapIcon
        {
            get => newBitmapIcon;
            set
            {
                newBitmapIcon = value;
                OnPropertyChanged();
            }
        }
        public byte[] NewIcon
        {
            get => newIcon;
            set
            {
                newIcon = value;
                OnPropertyChanged();
            }
        }

        public Models.SourceType SelectedSourceType
        {
            get => selectedSourceType;
            set
            {
                selectedSourceType = value;
                NewSource = string.Empty;
                OnPropertyChanged(nameof(SourceType));
                OnPropertyChanged(nameof(SelectedSourceType));
            }
        }

        public string SourceType => SelectedSourceType.ToString();

        public Array SourceTypes => Enum.GetValues(typeof(Models.SourceType));

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

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                FilterSources();
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


        public ObservableCollection<AccountGroupInfo> GroupedAccounts
        {
            get => groupedAccounts;
            set
            {
                groupedAccounts = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(MasterAccount user, string masterPassword)
        {
            CurrentUser = user;
            MasterPassword = masterPassword;
            DecryptAllPasswords(MasterPassword);
            GroupAccounts();
        }

        private void GroupAccounts()
        {
            if (GroupedAccounts != null)
                GroupedAccounts.Clear();

            var groupedData = CurrentUser.Accounts
                .GroupBy(a => a.Source)
                .OrderBy(g=>g.Key)
                .Select(group => new AccountGroupInfo(group.Key, new ObservableCollection<Account>(group.OrderBy(account=>account.Username))));

            GroupedAccounts = new ObservableCollection<AccountGroupInfo>(groupedData);
        }

        private int FindInsertionIndex(string source)
        {
            for (int i = 0; i < GroupedAccounts.Count; i++)
            {
                if (String.Compare(GroupedAccounts[i].Source, source, StringComparison.Ordinal) > 0)
                {
                    return i;
                }
            }
            return GroupedAccounts.Count;
        }

        private void AddAccountToGroup(Account newAccount)
        {
            var group = GroupedAccounts.FirstOrDefault(a => a.Source == newAccount.Source);
            if (group != null)
            {
                group.Accounts.Add(newAccount);
                group.SelectedAccount = newAccount;
            }
            else
            {
                var newGroup = new AccountGroupInfo(newAccount.Source, new ObservableCollection<Account> { newAccount });
                int index = FindInsertionIndex(newAccount.Source);
                GroupedAccounts.Insert(index, newGroup);
            }
        }

        private void UpdateAccountInGroup(Account updatedAccount)
        {
            foreach(var group in GroupedAccounts)
            {
                var account = group.Accounts.FirstOrDefault(a=> a.AccountId == updatedAccount.AccountId);
                if (account != null)
                {
                    account.Username = updatedAccount.Username;
                    account.EncryptedPassword = EncryptionService.Encrypt(NewPassword, MasterPassword, CurrentAccount.AccountSalt);
                    account.DecryptedPassword = EncryptionService.Decrypt(account.EncryptedPassword, MasterPassword, CurrentAccount.AccountSalt);
                    group.Accounts.Remove(account);
                    group.Accounts.Add(account);
                    group.SelectedAccount = account;
                    break;
                }
            }
        }

        private void DeleteAccountFromGroup(int accountId)
        {
            foreach(var group in GroupedAccounts)
            {
                var account = group.Accounts.FirstOrDefault(a=>a.AccountId == accountId);
                if (account != null)
                {
                    group.Accounts.Remove(account);
                    if (!group.Accounts.Any())
                    {
                        GroupedAccounts.Remove(group);
                    }
                    group.SelectedAccount = group.Accounts.FirstOrDefault();
                    break;
                }
            }
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

        
        public async Task InitializeAsync()
        {
            //for async operations upon initialization of mainviewmodel    
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
                    if(SelectedSourceType == Models.SourceType.Web)
                    {
                        ApplicationPath = null;
                        NewIcon = null;
                    }
                    else
                    {
                        NewIcon = ConvertBitmapImageToByteArray(NewBitmapIcon);
                    }

                    Account newUser = new Account
                    {
                        Username = username,
                        EncryptedPassword = password,
                        AccountSalt = generatedSalt,
                        AccountSourceType = SelectedSourceType,
                        Source = NewSource,
                        MasterAccountId = CurrentUser.UserId,
                        ApplicationIcon = NewIcon,
                        ApplicationPath = ApplicationPath
                    };
                    dbContext.Accounts.Add(newUser);
                    dbContext.SaveChanges();

                    //Adding to ObservableCollection<Account> Accounts for UI to reflect changes
                    newUser.DecryptedPassword = EncryptionService.Decrypt(newUser.EncryptedPassword, MasterPassword, newUser.AccountSalt);
                    CurrentUser.Accounts.Add(newUser);
                    AddAccountToGroup(newUser);
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


        private void EditAccountDialog(object param)
        {
            CurrentAccount = param as Account;
            NewUsername = CurrentAccount.Username;
            NewPassword = CurrentAccount.DecryptedPassword;
            CurrentDialog = "EditAccountTemplate";
            IsDialogOpen = true;
        }

        private void CloseEditAccountDialog()
        {
            IsDialogOpen = false;
            ResetProperties();
        }

        private async void EditAccount()
        {
            try
            {
                using(DatabaseContext dbcontext  = new DatabaseContext())
                {
                    var accountToEdit = await dbcontext.Accounts.FindAsync(CurrentAccount.AccountId);
                    if(accountToEdit != null)
                    {
                        CurrentUser.Accounts.Remove(CurrentAccount);
                        accountToEdit.Username = NewUsername.ToLower();
                        accountToEdit.EncryptedPassword = EncryptionService.Encrypt(NewPassword, MasterPassword, CurrentAccount.AccountSalt);
                        accountToEdit.DecryptedPassword = EncryptionService.Decrypt(accountToEdit.EncryptedPassword, MasterPassword, CurrentAccount.AccountSalt);
                        CurrentUser.Accounts.Add(accountToEdit);
                        dbcontext.Accounts.Update(accountToEdit);
                        await dbcontext.SaveChangesAsync();
                        UpdateAccountInGroup(accountToEdit);
                    }
                    IsDialogOpen = false;
                    ResetProperties();
                    SetStatusText(false, $"Succesfully edited account \"{accountToEdit.Username}\" from {accountToEdit.Source}.");
                }
            }
            catch (Exception ex)
            {
                SetStatusText(true, $"Error: {ex.Message}");
            }
        }

        private void DeleteAccount()
        {
            IsDialogOpen = false;
            if (CurrentAccount != null)
            {
                using (var databaseContext = new DatabaseContext())
                {
                    var accountToDelete = databaseContext.Accounts.Find(CurrentAccount.AccountId);
                    if (accountToDelete != null)
                    {
                        databaseContext.Accounts.Remove(accountToDelete);
                        databaseContext.SaveChanges();
                        CurrentUser.Accounts.Remove(accountToDelete);
                        DeleteAccountFromGroup(accountToDelete.AccountId);
                    }
                }
                DialogHost.CloseDialogCommand.Execute(null, null);
                SetStatusText(false, $"Successfully deleted account \"{CurrentAccount.Username}\" from {CurrentAccount.Source}.");
            }
        }

        private void DeleteAccountDialog(object param)
        {
            CurrentAccount = param as Account;
            CurrentDialog = "ConfirmDeleteTemplate";
            IsDialogOpen = true;
        }


        private void SignOut(object param)
        {
            windowService.ShowWindow(new LoginView(), new LoginViewModel());
            var window = param as MainView;
            windowService.CloseWindow(window);
        }

        private bool AllFieldsCompleted()
        {
            return !string.IsNullOrEmpty(NewUsername) && !string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(NewSource);
        }

        private bool AllEditFieldsCompleted()
        {
            return (NewUsername != CurrentAccount.Username || NewPassword != CurrentAccount.DecryptedPassword) 
                && (!string.IsNullOrEmpty(NewUsername) && !string.IsNullOrEmpty(NewPassword));
        }

        private void CopyText(object param)
        {
            if(param != null)
            {
                string textToCopy = param as string;
                Clipboard.SetText(textToCopy);
            }
        }


        private void DecryptAllPasswords(string masterPassword)
        {
            foreach(Account acc in CurrentUser.Accounts)
            {
                acc.DecryptedPassword = EncryptionService.Decrypt(acc.EncryptedPassword, masterPassword, acc.AccountSalt);
            }
        }

        private void SetStatusText(bool isError, string text)
        {
            StatusBarIcon = isError ? MaterialDesignThemes.Wpf.PackIconKind.ErrorOutline :
            MaterialDesignThemes.Wpf.PackIconKind.CheckOutline;
            StatusBarText = text;
            StatusBarColor = isError ? new SolidColorBrush(Colors.Orange) : new SolidColorBrush(Colors.Teal);
        }

        public void ResetProperties()
        {
            SearchText = "";
            NewUsername = "";
            NewPassword = "";
            NewSource = "";
            NewIcon = null;
            ApplicationPath = "";
        }

        private void OpenLink(object param)
        {
            if (param != null)
            {
                CurrentAccount = param as Account;
                try
                {
                    if(CurrentAccount.AccountSourceType == 0)
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = CurrentAccount.Source,
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                    else
                    {
                        Process.Start(CurrentAccount.ApplicationPath);
                    }
                }
                catch (Exception ex)
                {
                    SetStatusText(true, $"Error: {ex.Message}");
                }
            }
        }

        private void SelectApplication()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Applications (*.exe)|*.exe|All files (*.*)|*.*",
                Title = "Select an Application"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ApplicationPath = openFileDialog.FileName;
                string appName = Path.GetFileNameWithoutExtension(ApplicationPath);

                // Store the application name in a property
                NewSource = appName; // Assuming ApplicationName is a property in your ViewModel

                // Extract and store the icon
                Icon extractedIcon = Icon.ExtractAssociatedIcon(ApplicationPath);
                if (extractedIcon != null)
                {
                    using (MemoryStream iconStream = new MemoryStream())
                    {
                        extractedIcon.ToBitmap().Save(iconStream, ImageFormat.Png);
                        iconStream.Seek(0, SeekOrigin.Begin);

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = iconStream;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        NewBitmapIcon = bitmapImage; // Assuming ApplicationIcon is a property of type BitmapImage
                    }
                }
            }
        }

        private void FilterSources()
        {
            if (String.IsNullOrEmpty(SearchText))
            {
                GroupAccounts();
                return;
            }
            else
            {
                GroupAccounts();
                List<AccountGroupInfo> groupsToRemove = new List<AccountGroupInfo>();
                foreach(var group in GroupedAccounts)
                    if (!group.Source.Contains(SearchText))
                        groupsToRemove.Add(group);
                foreach (var group in groupsToRemove)
                    GroupedAccounts.Remove(group);
            }
        }

        public byte[] ConvertBitmapImageToByteArray(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
            {
                return null;
            }

            byte[] data;
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }
    }
}
