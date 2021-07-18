using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext dbContext;
        private readonly ITokenService tokenService;

        public AccountController(DataContext dbContext,ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            using (var hmac=new HMACSHA512())
            {
                if(await UserExists(registerDTO.Username))return BadRequest("Username is taken.");
                var user=new AppUser
                {
                UserName=registerDTO.Username.ToLower(),
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt=hmac.Key
                };

                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();

                return new UserDTO{
                    Username=registerDTO.Username,
                    Token=tokenService.CreateToken(user)
                };
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user=await dbContext.Users.SingleOrDefaultAsync(x=>x.UserName==loginDTO.Username);

            if(user==null)return Unauthorized("Invalid Username");

            using (var hmac=new HMACSHA512(user.PasswordSalt))
            {
                var ComputeHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
                for (int i = 0; i < ComputeHash.Length; i++)
                {
                    if(ComputeHash[i]!=user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
                }

            }
             return new UserDTO{
                    Username=loginDTO.Username,
                    Token=tokenService.CreateToken(user)
                };
        }

        private async Task<bool> UserExists(string username)
        {
            return await dbContext.Users.AnyAsync(x=>x.UserName==username);

        }
    }
}