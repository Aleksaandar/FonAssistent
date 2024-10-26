using FONAssistant.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using FONAssistant.Shared.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IMPOLAssistant.API.Controllers
{
   
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly FonAssistentDbContext _context;
        public AuthController(FonAssistentDbContext context)
        {
            _context = context;
        }
        private User HandleLogin(UserLogin login)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);
            return user;

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin login)
        {
          
            var user = HandleLogin(login);

            if (user!=null)
            {
                var token = GenerateJwtToken(login.Username, user.Role,user.Ime,user.Prezime);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username, string role, string firstName, string lastName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Is7NqOS2LTt8fg6fjBEhHa0LWlrq2pyL"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("FonAsistent",
              "FonAudience",
              new List<Claim>()
              {
                     new Claim(ClaimTypes.Name, username),
                     new Claim(ClaimTypes.Role, role),
                        new Claim("FirstName", firstName),
                    new Claim("LastName", lastName)

              },
              expires: DateTime.Now.AddMinutes(180),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
