using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
         public AuthRepository(DataContext context)
         {
             _context=context;
         }
        public async Task<User> Login(string username, string password)
        {
            var user=await _context.User.FirstOrDefaultAsync(x=>x.username==username);

            if (user==null)
            return null;

            if(!VerifyPasswordHash(password,user.passwordSalt,user.passwordHash))
            return null;

            return user;
        }

        private bool VerifyPasswordHash(string password,byte[] passwordSalt,byte[] passwordHash)
        {
             using (var hmac= new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {

                for (int i = 0; i < hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Length; i++)
            {
                if(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))[i] != passwordHash[i] )
                return false;
            }
            
            }
            return true; 
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordSlat, passwordHash;
            CreatHashPassword(password,out passwordSlat,out passwordHash);
            user.passwordSalt=passwordSlat;
            user.passwordHash=passwordHash;
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();  
            return user;
        }

        private void CreatHashPassword(string password, out byte[] passwordSlat, out byte[] passwordHash)
        {
            using (var hmac= new System.Security.Cryptography.HMACSHA512())
            {
            passwordSlat=hmac.Key;
            passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); 
            } 

        }

        public async Task<bool> UserExistes(string username)
        {
            if (await _context.User.AnyAsync(x=>x.username==username))
            return true;

            return false;
        }
    }
}