using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_manager.Models
{

    public class AccountGroupInfo : BaseModel
    {
        private Account selectedAccount;

        public AccountGroupInfo(string source,ObservableCollection<Account> accounts)
        {
            this.Source = source;
            this.Accounts = accounts;
            this.AccountSourceType = Accounts.First().AccountSourceType;
            this.ApplicationIcon = Accounts.First().ApplicationIcon;
            this.SelectedAccount = Accounts.First();
        }

        public string Source { get; set; }
        public SourceType AccountSourceType { get; set; }
        public ObservableCollection<Account> Accounts { get; set; }
        public Account SelectedAccount
        {
            get => selectedAccount;
            set
            {
                selectedAccount = value;
                OnPropertyChanged();
            }
        }
        public byte[] ApplicationIcon { get; set; }
    }
}
