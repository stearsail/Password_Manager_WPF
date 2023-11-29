using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Password_manager.Base;
using System.Windows.Media.Imaging;

namespace Password_manager.Models
{
    public class Account
    {
        public enum SourceType
        {
            Web,
            Application
        }

        public int AccountId { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public string AccountSalt { get; set; }
        [Required]public SourceType AccountSourceType { get; set; }
        public string Source { get; set; }
        public int MasterAccountId { get; set; }
        public MasterAccount MasterAccount { get; set; }
        [NotMapped]
        public string DecryptedPassword { get; set; }
        public byte[] ApplicationIcon { get; set; }
    }
}
