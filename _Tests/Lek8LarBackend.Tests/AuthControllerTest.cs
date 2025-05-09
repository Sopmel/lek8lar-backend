using Lek8LarBackend.Controllers;
using Lek8LarBackend.Data;
using Lek8LarBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Lek8LarBackend.Tests.Lek8LarBackend.Tests
{
    public class AuthControllerTest
    {
        private readonly AuthController _controller;
        private readonly ApplicationDbContext _context;
        private readonly SymmetricSecurityKey _testKey;

        public AuthControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new ApplicationDbContext(options);

            var keyBytes = Encoding.UTF8.GetBytes("TestSecretKeyForUnitTestsOnly123456"); // Ensure key is at least 32 bytes
            _testKey = new SymmetricSecurityKey(keyBytes);

            _controller = new AuthController(_context, _testKey);
        }

        [Fact]
        public void Register_ReturnsBadRequest_WhenUsernameIsTaken()
        {
            var existingUser = new User { Username = "TestUser", PasswordHash = "hashedpassword", Role = "User" };
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var request = new RegisterRequest { Username = "TestUser", Password = "password" };

            var result = _controller.Register(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var actualMessage = badRequestResult.Value?.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value, null);
            Assert.Equal("Användarnamnet är upptaget.", actualMessage);
        }

        [Fact]
        public void Login_ReturnsOk_WithValidCredentials()
        {
            var existingUser = new User { Username = "TestUser", PasswordHash = HashPassword("password"), Role = "User" };
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var request = new LoginRequest { Username = "TestUser", Password = "password" };
            var result = _controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value?.GetType().GetProperty("Token")?.GetValue(okResult.Value, null);
            Assert.NotNull(response);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WithInvalidCredentials()
        {
            var existingUser = new User { Username = "ValidUser", PasswordHash = HashPassword("validpassword"), Role = "User" };
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            var request = new LoginRequest { Username = "ValidUser", Password = "wrongpassword" };

            var result = _controller.Login(request);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var actualMessage = unauthorizedResult.Value?.GetType().GetProperty("Message")?.GetValue(unauthorizedResult.Value, null);
            Assert.Equal("Felaktigt användarnamn eller lösenord.", actualMessage);
        }
    }
}
