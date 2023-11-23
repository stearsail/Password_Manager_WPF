using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Password_manager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_manager
{
    public class DatabaseContext : DbContext
    {
        public DbSet<MasterAccount> MasterAccounts { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PasswordManagerDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MasterAccount>()
                .HasIndex(m => m.Username)
                .IsUnique();
            builder.Entity<MasterAccount>()
                .HasIndex(m => m.Email)
                .IsUnique();
        }

        public MasterAccount GetMasterAccountByUsername(string username)
        {
            var masterAccount =  MasterAccounts.Include(macc => macc.Accounts).ToList().FirstOrDefault(macc => macc.Username == username);
            if (masterAccount != null && masterAccount.Accounts == null)
            {
                masterAccount.Accounts = new ObservableCollection<Account>(Accounts.Where(a => a.MasterAccountId == masterAccount.UserId));
            }
            return masterAccount;

        }
    }
}
