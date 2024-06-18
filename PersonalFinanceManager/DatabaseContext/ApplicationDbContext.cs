using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Models;

namespace PersonalFinanceManager.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.FinancialAccounts)
                .WithOne(fa => fa.User)
                .HasForeignKey(fa => fa.UserId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.DebitedAccount)
                .WithMany(fa => fa.OutgoingTransactions)
                .HasForeignKey(t => t.DebitedAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.CreditedAccount)
                .WithMany(fa => fa.IncomingTransactions)
                .HasForeignKey(t => t.CreditedAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }


        public DbSet<User> Users { get; set; }
        public DbSet<FinancialAccount> FinancialAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
