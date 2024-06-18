using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace PersonalFinanceManager.Models
{
    public class Transaction
    {
        [Key] public int TransactionId { get; set; }
        [Required] public int AmountCents { get; set; }
        [Required] public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();

        [JsonConverter(typeof(StringEnumConverter))]
        [Required] public TransactionCategory Category { get; set; } 
        public string? TransactionSubcategory { get; set; }

        // Foreign keys
        public int? DebitedAccountId { get; set; }
        public int? CreditedAccountId { get; set; }

        // Navigation properties
        public FinancialAccount? DebitedAccount;
        public FinancialAccount? CreditedAccount;

        public Transaction() { }
        public Transaction(CreateExpenseTransactionDTO dto) 
        {
            AmountCents = dto.AmountCents;
            Description = dto.Description;
            Timestamp = dto.Timestamp;
            Category = TransactionCategory.Expense;
            TransactionSubcategory = dto.ExpenseSubcategory.ToString();
            CreditedAccountId = dto.AccountId;
        }
        public Transaction(CreateIncomeTransactionDTO dto) 
        {
            AmountCents = dto.AmountCents;
            Description = dto.Description;
            Timestamp = dto.Timestamp;
            Category = TransactionCategory.Income;
            TransactionSubcategory = dto.IncomeSubcategory.ToString();
            DebitedAccountId = dto.AccountId;
        }
        public Transaction(CreateInternalTransferTransactionDTO dto) 
        {
            AmountCents = dto.AmountCents;
            Description = dto.Description;
            Timestamp = dto.Timestamp;
            Category = TransactionCategory.Transfer;
            CreditedAccountId = dto.SourceCreditedAccountId;
            DebitedAccountId = dto.DestinyDebitedAccountId;
        }
    }

    public enum TransactionCategory
    {
        Expense,
        Income,
        Transfer,
    }

    public enum ExpenseSubcategory
    {
        Other,
        Food,
        Utilities,
        Rent,
        Transportation,
        Entertainment,
        Clothing,
        Education,
        Healthcare,
        PersonalCare,
        Gifts
    }

    public enum IncomeSubcategory
    {
        Other,
        Salary,
        Bonus,
        Investments,
        RentalIncome,
        FreelanceIncome,
        Interest,
        Dividends,
        Grants,
        Gifts,
    }

    public class CreateIncomeTransactionDTO
    {
        [Range(0, int.MaxValue, ErrorMessage = "AmountCents must be a non-negative number.")]
        [Required] public int AmountCents { get; set; }
        [Required] public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();

        [JsonConverter(typeof(StringEnumConverter))]
        [Required] public IncomeSubcategory IncomeSubcategory { get; set; }
        [Required] public int AccountId { get; set; }
    }

    public class CreateExpenseTransactionDTO
    {
        [Range(0, int.MaxValue, ErrorMessage = "AmountCents must be a non-negative number.")]
        [Required] public int AmountCents { get; set; }
        [Required] public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();

        [JsonConverter(typeof(StringEnumConverter))]
        [Required] public ExpenseSubcategory ExpenseSubcategory { get; set; }
        [Required] public int AccountId { get; set; }
    }

    public class CreateInternalTransferTransactionDTO
    {
        [Range(0, int.MaxValue, ErrorMessage = "AmountCents must be a non-negative number.")]
        [Required] public int AmountCents { get; set; }
        [Required] public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();
        [Required] public int SourceCreditedAccountId { get; set; }
        [Required] public int DestinyDebitedAccountId { get; set; }
    }

}

