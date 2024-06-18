using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Utils;
using PersonalFinanceManager.DatabaseContext;
using PersonalFinanceManager.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PersonalFinanceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null) return Utils.Response.NotFound(this);
            
            return Ok(transaction);
        }

        [HttpPost("income")]
        [SwaggerOperation(
            Summary = "Cria uma nova conta transação de receita, que afeta uma conta financeira do usuário, aumentando o saldo."
        )]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(Transaction))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized,type: typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden,type: typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound,type: typeof(ProblemDetails))]
        public async Task<ActionResult<Transaction>> PostIncomeTransaction(CreateIncomeTransactionDTO dto)
        {
            var (user, ok) = await Auth.GetUserByAuth(HttpContext, _context);
            if (!ok) return Utils.Response.Unauthorized(this);
            if (!AccountExists(dto.AccountId).Result) return Utils.Response.NotFound(this, $"The financial account w/ id {dto.AccountId} doesn't exist ");
            
            var userFinancialAccounts = _context.FinancialAccounts.Where(acc=>acc.UserId == user.Id).ToList();
            if (userFinancialAccounts.All((account)=>account.Id != dto.AccountId))
            {
                return Utils.Response.Forbidden(this,detail: $"The financial account w/ id {dto.AccountId} doesn't belong to you!!");
            }
            var transaction = new Transaction(dto);
            _context.Transactions.Add(transaction);

            transaction.DebitedAccount = await _context.FinancialAccounts.FindAsync(dto.AccountId);
            transaction.DebitedAccount!.BalanceCents += dto.AmountCents;
            

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }
        

        [HttpPost("expense")]
        [SwaggerOperation(
            Summary = "Cria Uma nova conta transação de despesa, que afeta uma conta financeira do usuário,diminuindo o saldo."
        )]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(Transaction))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized,type: typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, type: typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, type: typeof(ProblemDetails))]


        public async Task<ActionResult<Transaction>> PostExpenseTransaction(CreateExpenseTransactionDTO dto)
        {
            var (user, ok) = await Auth.GetUserByAuth(HttpContext, _context);
            if (!ok) return Utils.Response.Unauthorized(this);
            if (!AccountExists(dto.AccountId).Result) return Utils.Response.NotFound(this, $"The financial account w/ id {dto.AccountId} doesn't exist ");
            
            var userFinancialAccounts = _context.FinancialAccounts.Where(acc => acc.UserId == user.Id).ToList();
            if (userFinancialAccounts.All((account) => account.Id != dto.AccountId))
            {
                return Utils.Response.Forbidden(this, detail: $"The financial account w/ id {dto.AccountId} doesn't belong to you!!");
            }
            var transaction = new Transaction(dto);
            _context.Transactions.Add(transaction);

            transaction.CreditedAccount = await _context.FinancialAccounts.FindAsync(dto.AccountId);
            transaction.CreditedAccount!.BalanceCents -= dto.AmountCents;


            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }



        [HttpPost("internal-transfer")]
        [SwaggerOperation(
            Summary = "Cria Uma nova conta transação de transferncias interna.",
            Description = "Cria Uma nova conta transação de transferncias interna, que afeta duas contas financeiras do usuário,diminuindo o saldo de uma e aumenta o salda da outra."
        )]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(Transaction))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, type: typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, type: typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, type: typeof(ProblemDetails))]
        public async Task<ActionResult<Transaction>> PostInternalTransferTransaction(CreateInternalTransferTransactionDTO dto)
        {
            var (user, ok) = await Auth.GetUserByAuth(HttpContext, _context);
            if (!ok) return Utils.Response.Unauthorized(this);

            if (!AccountExists(dto.DestinyDebitedAccountId).Result) return Utils.Response.NotFound(this);

            user.FinancialAccounts = _context.FinancialAccounts.Where(acc => acc.UserId == user.Id).ToList();
            
            if (user.FinancialAccounts.Count((acc)=>acc.Id == (dto.SourceCreditedAccountId) || acc.Id == (dto.DestinyDebitedAccountId)) != 2)
            {
                return Utils.Response.NotFound(this,detail: $"Some of the financial accounts (ids {dto.SourceCreditedAccountId} and {dto.DestinyDebitedAccountId} don't exist!!");
            }


            // if all user's financial accounts are neither the debited acc nor the credit acc 
            if (user.FinancialAccounts.All((account) => (account.Id != dto.SourceCreditedAccountId && account.Id != dto.DestinyDebitedAccountId)))
            {
                return Utils.Response.Forbidden(this,
                    detail: $"Some of the financial accounts (ids {dto.SourceCreditedAccountId} and {dto.DestinyDebitedAccountId} don't belong to you!!"
                );
            }
            var transaction = new Transaction(dto);
            _context.Transactions.Add(transaction);


            transaction.CreditedAccount = await _context.FinancialAccounts.FindAsync(dto.SourceCreditedAccountId);
            transaction.DebitedAccount = await _context.FinancialAccounts.FindAsync(dto.DestinyDebitedAccountId);

            transaction.DebitedAccount!.BalanceCents += dto.AmountCents;
            transaction.CreditedAccount!.BalanceCents -= dto.AmountCents;

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }

        private async Task<bool> AccountExists(int id) 
        {
            return await _context.FinancialAccounts.AnyAsync((acc) => acc.Id == id);
        }
    }
    
}
