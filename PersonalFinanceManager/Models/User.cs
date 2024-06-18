using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.DatabaseContext;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public DateTime PasswordLastModifiedAt {  get; set; } = DateTime.Now.ToUniversalTime();
        // Navigation property
        public ICollection<FinancialAccount> FinancialAccounts { get; set; } = new List<FinancialAccount>();

        public User(){}
        public User(CreateUserRequestDTO dto,byte[] passwordHash, byte[] passwordSalt)
        {
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            Email = dto.Email;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
        }


    }

    public class CreateUserRequestDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;



    }

    public class UserResponseDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool IsEmailVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime PasswordLastModifiedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public ICollection<FinancialAccountSimpleResponse> FinancialAccounts { get; set; } = new List<FinancialAccountSimpleResponse>();
        public int TotalBalanceCents { get; set; } = 0;

        public UserResponseDTO() { }
        public UserResponseDTO(User user,ApplicationDbContext dbCtx) 
        {


            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            PasswordLastModifiedAt = user.PasswordLastModifiedAt;
            CreatedAt = user.CreatedAt;
            IsEmailVerified = user.IsEmailVerified;
            FinancialAccounts = dbCtx.FinancialAccounts
                .Where(acc => acc.UserId == user.Id)
                .Select(acc => new FinancialAccountSimpleResponse(acc)).ToList();
            TotalBalanceCents = FinancialAccounts.Aggregate(0, (accum, account) => accum + account.BalanceCents);
        }
    }

}
    