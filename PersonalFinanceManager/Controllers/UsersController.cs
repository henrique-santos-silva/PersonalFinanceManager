
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.DatabaseContext;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace PersonalFinanceManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("me")]
        [SwaggerOperation(
            Summary = "Retorna o Usuario atual",
            Description = "Usuario atual baseado no Header 'Authorization'."
        )]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK,type:typeof(UserResponseDTO))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized,Utils.Response.DEFAULT_UNAUTHORIZED_DETAIL, typeof(ProblemDetails))]
        public async Task<ActionResult<User>> GetUser()
        {
            var (user, ok) = await Auth.GetUserByAuth(HttpContext, _context);
            if (!ok) return Utils.Response.Unauthorized(this);
            

            var accounts = await _context.FinancialAccounts
                                 .Where(fa => fa.UserId == user!.Id)
                                 .ToListAsync();
            var responseDTO = new UserResponseDTO(user, _context);
            return Ok(responseDTO);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria um novo usuário",
            Description = "Cria um novo usuário, com o atributo ÚNICO email. Único endpoint que não requer autenticação."
        )]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(UserResponseDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest,Utils.Response.DEFAULT_BADREQUEST_DETAIL, type: typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, Utils.Response.DEFAULT_FORBIDDEN_DETAIL, typeof(ProblemDetails))]
        public async Task<ActionResult<User>> PostUser(CreateUserRequestDTO dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>() {
                    { "Email", new string[] { $"Email '{dto.Email}' already exists."} } }
                ));
            }


            byte[] salt = Auth.GenerateRandomSalt();
            byte[] pwdHash = Auth.GeneratePasswordHash(dto.Password, salt);

            var user = new User(dto,passwordHash:pwdHash,passwordSalt:salt);
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Created("", new UserResponseDTO(user,_context));
        }
    }
}
