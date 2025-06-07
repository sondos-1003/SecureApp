using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureWebApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SecureWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if user exists by email or username
            var userByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            var userByName = await _userManager.FindByNameAsync(registerDto.Username);

            if (userByEmail != null || userByName != null)
                return Conflict("Username or email already exists");

            // Create new user
            var user = new IdentityUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            // Create user with password
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Add role claim
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, registerDto.Role));

            _logger.LogInformation($"New user registered: {registerDto.Username} ({registerDto.Role})");

            return Ok(new
            {
                Username = user.UserName,
                Email = user.Email,
                Role = registerDto.Role,
                Token = GenerateJwtToken(user, registerDto.Role)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find user by username (as per your LoginDto)
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Unauthorized("Invalid username or password");

            // Get user's role claim
            var claims = await _userManager.GetClaimsAsync(user);
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var role = roleClaim?.Value ?? "User";

            _logger.LogInformation($"User logged in: {user.UserName}");

            return Ok(new
            {
                Username = user.UserName,
                Email = user.Email,
                Role = role,
                Token = GenerateJwtToken(user, role)
            });
        }

        private string GenerateJwtToken(IdentityUser user, string role)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    authSigningKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}