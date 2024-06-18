using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.DatabaseContext;
using PersonalFinanceManager.Models;
using System.Security.Cryptography;
using System.Text;

namespace PersonalFinanceManager.Utils
{
    public static class Auth
    {
        public static byte[] GenerateRandomSalt()
        {
            return RandomNumberGenerator.GetBytes(16);
        }
        

        public static byte[] GeneratePasswordHash(string password, byte[] passwordSalt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            
            byte[] saltedPassword = new byte[passwordBytes.Length + passwordSalt.Length];
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, password.Length);
            Buffer.BlockCopy(passwordSalt, 0, saltedPassword, password.Length, passwordSalt.Length);

            byte[] hashedPassword = SHA256.HashData(saltedPassword);
            return hashedPassword;
        }

        public static bool ValidadePassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            byte[] expectedPasswordHash = GeneratePasswordHash(password, passwordSalt);
            if (expectedPasswordHash.Length != passwordHash.Length)
                return false;
            
            for (int i = 0; i < passwordHash.Length; i++) 
                if (passwordHash[i] != expectedPasswordHash[i])
                    return false;

            return true;
        }

        public static async Task<(User?, bool)> GetUserByAuth(HttpContext ctx, ApplicationDbContext dbCtx)
        {
            if (!ctx.Request.Headers.ContainsKey("Authorization")) return (null, false);

            var authHeader = ctx.Request.Headers["Authorization"].ToString();
            if (!authHeader.StartsWith("Basic ")) return (null, false);

            var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials)).Split(':');
            if (decodedCredentials.Length != 2) return (null, false);

            var email = decodedCredentials[0];
            var password = decodedCredentials[1];
            return await ValidateUserCredentials(email, password, dbCtx);
        }

        public static async  Task<(User?, bool)>  ValidateUserCredentials(string email, string password, ApplicationDbContext dbCtx)
        {
            User? candidate = await dbCtx.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (candidate == null) {  return (null, false); }

            if (ValidadePassword(password, candidate.PasswordHash,candidate.PasswordSalt))
            {
                return (candidate, true);
            }
            return (null, false);
        }
    }
}
