using AuthService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AuthService.Api.Services;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using AuthService.Api.Repositories;
using Kafka.Contracts.Messages;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IKafkaProducerService _kafkaProducer;

        public AuthController(IUserRepository userRepository, IConfiguration config, IKafkaProducerService kafkaProducer)
        {
            _userRepository = userRepository;
            _config = config;
            _kafkaProducer = kafkaProducer;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userRepository.EmailExistsAsync(dto.Email))
                return Conflict("Email already registered.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordService.HashPassword(dto.Password)
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = GenerateEmailConfirmationJwt(user);
            await _kafkaProducer.ProduceUserRegisteredAsync(new UserRegisteredMessage
            {
                Email = user.Email,
                Token = token
            });

            return CreatedAtAction(nameof(Register), new { id = user.UserId }, new { user.UserId, user.Username, user.Email });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password");

            if (!PasswordService.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password");

            var token = GenerateJwt(user);
            return Ok(new LoginResponseUserDto { Token = token });
        }

        // GET: api/auth/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = (await _userRepository.GetAllUsersAsync())
            .Select(u => new ResponseUserDto 
            { 
                UserId = u.UserId, 
                Username = u.Username, 
                Email = u.Email, 
                IsEmailVerified = u.IsEmailVerified 
            });

            return Ok(users);
        }

        // GET: api/auth/users/{id}
        [HttpGet("users/{id:guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new ResponseUserDto 
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified
            });
        }

        // PUT: api/auth/users/{id}
        [HttpPut("users/{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = dto.Email ?? user.Email;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = PasswordService.HashPassword(dto.Password);
            }

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/auth/users/{id}
        [HttpDelete("users/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepository.DeleteUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token is required.");

            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var tokenType = principal.FindFirst("token_type")?.Value;
                if (tokenType != "email_confirmation")
                    return BadRequest("Invalid token type.");

                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest("Invalid token payload.");

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return BadRequest("Invalid user ID in token.");
                }

                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound("User not found.");

                if (user.IsEmailVerified)
                    return Ok("Email already confirmed.");

                user.IsEmailVerified = true;
                await _userRepository.SaveChangesAsync();

                return Ok("Email confirmed successfully.");
            }
            catch (SecurityTokenExpiredException)
            {
                return BadRequest("Token has expired.");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token.");
            }
        }

        private string GenerateJwt(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateEmailConfirmationJwt(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("token_type", "email_confirmation")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}