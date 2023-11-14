using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Password_manager.Models;
using System;
using System.Collections.Generic;
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
    }
}
