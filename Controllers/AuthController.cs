using Lek8LarBackend.Data;
using Lek8LarBackend.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Lek8LarBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SymmetricSecurityKey _key;
        public AuthController(ApplicationDbContext context, SymmetricSecurityKey key)
        {
            _context = context;
            _key = key;
        }



        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { Message = "Användarnamn och lösenord krävs." });
            }

      
            if (_context.Users.Any(u => u.Username == request.Username))
             return BadRequest(new { Message = "Användarnamnet är upptaget." });

            var passwordHash = HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { Message = "Registrering Lyckades!", Role = user.Role });
            }
        [HttpPost("login")]

        public IActionResult Login([FromBody] LoginRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { Message = "Användarnamn och lösenord krävs." });

            var user = _context.Users.SingleOrDefault(u => u.Username == request.Username);
            if(user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized(new { Message = "Felaktigt användarnamn eller lösenord." });

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, Username = user.Username });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }

    public class RegisterRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
