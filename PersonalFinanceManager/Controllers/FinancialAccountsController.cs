using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Utils;
using PersonalFinanceManager.DatabaseContext;
using PersonalFinanceManager.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PersonalFinanceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialAccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FinancialAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        
        [SwaggerOperation(
            Summary = "Obtem as contas do usuario",
            Description = "Obtem as contas do usuari baseado no Header 'Authorization'."
        )]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<FinancialAccount>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Utils.Response.DEFAULT_UNAUTHORIZED_DETAIL, typeof(ProblemDetails))]
        public async Task<ActionResult<IEnumerable<FinancialAccount>>> GetFinancialAccounts()
        {
            var (user, ok) = await Auth.GetUserByAuth(HttpContext, _context);
            if (!ok) return Utils.Response.Unauthorized(this);

            var accounts = await _context.FinancialAccounts.Where(fa => fa.UserId == user!.Id).ToListAsync();
            var dtos = accounts.Select(acc => new FinancialAccountDetailedResponse(acc, _context));

            return Ok(dtos);
        }

        
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtem os detalhes de uma conta, por ID",
            Description = "Obtem os detalhes de uma conta financeira especifica que pertence ao usuario."
        )]
        [Produces("application/json")]

        [SwaggerResponse(StatusCodes.Status200OK,type:typeof(FinancialAccount))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Utils.Response.DEFAULT_UNAUTHORIZED_DETAIL, typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, Utils.Response.DEFAULT_FORBIDDEN_DETAIL, typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound,Utils.Response.DEFAULT_NOTFOUND_DETAIL,typeof(ProblemDetails))]
        public async Task<ActionResult<FinancialAccount>> GetFinancialAccount(int id)
        {
            var (user, ok) = await Auth.GetUserByAuth(HttpContext, _context);

            if (!ok) return Utils.Response.Unauthorized(this);
            
            var financialAccount = await _context.FinancialAccounts.FindAsync(id);
            if (financialAccount == null) return Utils.Response.NotFound(this);

            if (financialAccount.UserId != user!.Id) return Utils.Response.Forbidden(this);

            return Ok(new FinancialAccountDetailedResponse(financialAccount,_context));
        }


        [HttpPost()]
        [SwaggerOperation(
            Summary = "Cria Uma nova conta financeira pertencente ao usuário"
        )]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(FinancialAccount))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Utils.Response.DEFAULT_BADREQUEST_DETAIL, type: typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Utils.Response.DEFAULT_UNAUTHORIZED_DETAIL, typeof(ProblemDetails))]
        public async Task<ActionResult<FinancialAccount>> PostFinancialAccount(CreateFinancialAccountRequest accDTO)
        {
            var (user, ok) = await Auth.GetUserByAuth(HttpContext, _context);
            if (!ok) return Utils.Response.Unauthorized(this); 
            
            var financialAcc = new FinancialAccount()
            {
                AccountName = accDTO.AccountName,
                UserId = user.Id
            };

            _context.FinancialAccounts.Add(financialAcc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFinancialAccount", new { id = financialAcc.Id }, financialAcc);
        }




    }
}
