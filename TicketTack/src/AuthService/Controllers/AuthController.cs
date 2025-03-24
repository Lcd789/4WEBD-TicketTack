using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TicketTack.Shared.DTOs;
using TicketTack.Shared.Infrastructure.MongoDB;
using TicketTack.Shared.Models;
using BC = BCrypt.Net.BCrypt;

namespace TicketTack.AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IMongoRepository<User> userRepository,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register(RegisterUserDto registerDto)
        {
            try
            {
                // Vérifier si l'email existe déjà
                var existingUser = await _userRepository.FindAsync(u => u.Email == registerDto.Email);
                if (existingUser.Any())
                {
                    return BadRequest("Email already in use");
                }

                // Créer un nouvel utilisateur
                var user = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    PasswordHash = BC.HashPassword(registerDto.Password),
                    PhoneNumber = registerDto.PhoneNumber,
                    Role = "User", // Par défaut, le rôle est "User"
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);

                // Générer des tokens
                var (accessToken, refreshToken, expiresAt) = GenerateTokens(user);

                return Ok(new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    TokenType = "Bearer"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginDto)
        {
            try
            {
                // Rechercher l'utilisateur par email
                var users = await _userRepository.FindAsync(u => u.Email == loginDto.Email);
                var user = users.FirstOrDefault();

                // Vérifier si l'utilisateur existe et si le mot de passe est correct
                if (user == null || !BC.Verify(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid email or password");
                }

                // Générer des tokens
                var (accessToken, refreshToken, expiresAt) = GenerateTokens(user);

                return Ok(new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    TokenType = "Bearer"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user");
                return StatusCode(500, "Internal server error");
            }
        }

        private (string accessToken, string refreshToken, DateTime expiresAt) GenerateTokens(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("JWT configuration is incomplete");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id),
                new Claim("Email", user.Email),
                new Claim("Role", user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Access token expiration
            var expiresAt = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Générer un refresh token
            var refreshToken = Guid.NewGuid().ToString();

            return (accessToken, refreshToken, expiresAt);
        }
    }
}