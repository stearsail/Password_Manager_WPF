using System.ComponentModel.DataAnnotations.Schema;
using Password_manager.Base;

namespace Password_manager.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public string AccountSalt { get; set; }
        public string Source { get; set; }
        public int MasterAccountId { get; set; }
        public MasterAccount MasterAccount { get; set; }
        [NotMapped]
        public string DecryptedPassword { get; set; }
    }
}
