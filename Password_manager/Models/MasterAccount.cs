using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Password_manager.Models
{
    public class MasterAccount
    {
        [Key]public int UserId { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string PasswordHash { get; set; }
        [Required] public string Email { get; set; }
        public ObservableCollection<Account> Accounts { get; set;}
    }
}
