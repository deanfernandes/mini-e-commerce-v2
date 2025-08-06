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
using Microsoft.AspNetCore.Authorization;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly IJwtService _jwtService;

        public AuthController(IUserRepository userRepository, IKafkaProducerService kafkaProducer, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _kafkaProducer = kafkaProducer;
            _jwtService = jwtService;
        }

        // POST: api/auth/users
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userRepository.EmailExistsAsync(dto.Email))
                return Conflict("Email already registered.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordService.HashPassword(dto.Password),
                IsEmailVerified = true
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            var response = new ResponseUserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, response);
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

        // POST: api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
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

            var token = _jwtService.GenerateEmailConfirmationJwt(user);
            await _kafkaProducer.ProduceUserRegisteredAsync(new UserRegisteredMessage
            {
                Email = user.Email,
                Token = token
            });

            return CreatedAtAction(nameof(Register), new { id = user.UserId }, new { user.UserId, user.Username, user.Email });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password");

            if (!PasswordService.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password");

            if (!user.IsEmailVerified)
                return Unauthorized("Email is not verified");

            var token = _jwtService.GenerateLoginJwt(user);
            return Ok(new LoginResponseUserDto { Token = token });
        }

        // POST: api/auth/confirm
        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token is required.");

            ClaimsPrincipal? principal;
            try
            {
                principal = _jwtService.ValidateEmailConfirmationJwt(token);
                if (principal == null)
                    return BadRequest("Invalid token type or token.");
            }
            catch (SecurityTokenExpiredException)
            {
                return BadRequest("Token has expired.");
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid token payload.");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            if (user.IsEmailVerified)
                return Ok("Email already confirmed.");

            user.IsEmailVerified = true;
            await _userRepository.SaveChangesAsync();

            return Ok("Email confirmed successfully.");
        }
    }
}