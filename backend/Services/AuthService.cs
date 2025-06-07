using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecureWebApp.Data;
using SecureWebApp.Models;
using SecureWebApp.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureWebApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResult> Register(RegisterDto dto)
        {
            string encryptedUsername = AesEncryptionHelper.Encrypt(dto.Username);

            if (_context.Users.Any(u => u.Username == encryptedUsername))
                return new AuthResult(false, "Username exists");

            string hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                Username = encryptedUsername,
                PasswordHash = hashed,
                Email = dto.Email,
                Role = dto.Role
            };
            var testKey = "ThisIsMySuperSecureKey32BytesLong1234";
            var byteCount = Encoding.UTF8.GetByteCount(testKey);
            Console.WriteLine($"JWT Key Verification in AuthService:");
            Console.WriteLine($"Size: {byteCount} bytes, {byteCount * 8} bits");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResult(true, "Registered successfully");
        }

        public async Task<AuthResult> Login(LoginDto dto)
        {
            string encryptedUsername = AesEncryptionHelper.Encrypt(dto.Username);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == encryptedUsername);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new AuthResult(false, "Invalid credentials");

            string decryptedUsername = AesEncryptionHelper.Decrypt(user.Username);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, decryptedUsername),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Use the same 32-byte key from Program.cs
            var key = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes("ThisIsMySuperSecureKey32BytesLong1234"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "secureapi",
                audience: "secureapi",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new AuthResult(true, "Login successful", new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}