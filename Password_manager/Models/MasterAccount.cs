using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Password_manager.Models
{
    public class MasterAccount
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]public int UserId { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string PasswordHash { get; set; }
        [Required] public string PasswordSalt { get; set; }
        [Required] public string Email { get; set; }
        public ObservableCollection<Account> Accounts { get; set;}
    }
}
