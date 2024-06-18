using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.DatabaseContext;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class FinancialAccount
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string AccountName { get; set; } = string.Empty;

        [Required]
        public int BalanceCents { get; set; } = 0;
        public ICollection<Transaction> IncomingTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> OutgoingTransactions { get; set; } = new List<Transaction>();

        // Foreign key
        public int UserId;

        // Navigation property
        public User User;

    }






    public class CreateFinancialAccountRequest
    {
        [Required] public string AccountName { get; set; } = string.Empty;

    }


    public class FinancialAccountSimpleResponse
    {
        public int Id { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int BalanceCents { get; set; } = 0;

        public FinancialAccountSimpleResponse(FinancialAccount acc)
        {
            Id = acc.Id;
            AccountName = acc.AccountName;
            BalanceCents = acc.BalanceCents;
        }

    }


    public class FinancialAccountDetailedResponse
    {
        public int Id { get; set; }

        public string AccountName { get; set; } = string.Empty;
        public int BalanceCents { get; set; } = 0;
        public ICollection<Transaction> IncomingTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> OutgoingTransactions { get; set; } = new List<Transaction>();

        public FinancialAccountDetailedResponse(FinancialAccount acc, ApplicationDbContext dbCtx)
        {
            Id = acc.Id;
            AccountName = acc.AccountName;
            BalanceCents = acc.BalanceCents;
            IncomingTransactions = dbCtx.Transactions.Where(t => t.DebitedAccountId == acc.Id).ToListAsync().Result;
            OutgoingTransactions = dbCtx.Transactions.Where(t => t.CreditedAccountId == acc.Id).ToListAsync().Result;
        }

    }
}



