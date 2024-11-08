using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthController(UserService userService, JwtTokenGenerator tokenGenerator)
        {
            _userService = userService;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (!_userService.ValidateUser(user.Email, user.Password))
                return Unauthorized(new { message = "Email ou senha inválidos" });

            var tokenInfo = _tokenGenerator.GenerateToken(user.Email);

            Thread.Sleep(2000);

            return Ok(new
            {
                token = tokenInfo.Token,
                generatedAt = tokenInfo.GeneratedAt,
                expiresAt = tokenInfo.ExpiresAt,
                user = user.Email
            });
        }
    }

    public class UserService
    {
        private static readonly User DefaultUser = new User
        {
            Email = "usuario@cotiinformatica.com.br",
            Password = "@Admin2024"
        };

        public bool ValidateUser(string email, string password)
        {
            return email == DefaultUser.Email && password == DefaultUser.Password;
        }
    }

    public class User
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenInfo GenerateToken(string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expires = DateTime.Now.AddMinutes(30);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new TokenInfo
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                GeneratedAt = DateTime.Now,
                ExpiresAt = expires
            };
        }
    }

    public class TokenInfo
    {
        public string Token { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
