using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using SecureWebApp.backend.Services;
using SecureWebApp.Data;
using SecureWebApp.backend.Models;

namespace SecureWebApp.Services
{
    public class TotpMfaService : IMfaService
    {
        private readonly AppDbContext _context;
        private const int StepSeconds = 30; // Standard TOTP step (30 seconds)
        private const int Digits = 6; // Standard 6-digit code

        public TotpMfaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateCode(string userId)
        {
            // Get or create user's MFA secret
            var userMfa = await _context.UserMfaSecrets
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userMfa == null)
            {
                // Generate new secret if none exists
                var secretKey = KeyGeneration.GenerateRandomKey(20);
                userMfa = new UserMfaSecret
                {
                    UserId = userId,
                    SecretKey = secretKey,
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserMfaSecrets.Add(userMfa);
                await _context.SaveChangesAsync();
            }

            var totp = new Totp(userMfa.SecretKey, step: StepSeconds, totpSize: Digits);
            return totp.ComputeTotp();
        }

        public async Task<bool> VerifyCode(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false
                    ;

            var userMfa = await _context.UserMfaSecrets
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userMfa == null) return false;

            var totp = new Totp(userMfa.SecretKey, step: StepSeconds, totpSize: Digits);

            // Allow time window of ±1 step for clock drift
            long timeStepMatched;
            bool valid = totp.VerifyTotp(
                code,
                out timeStepMatched,
                window: new VerificationWindow(previous: 1, future: 1));

            return valid;
        }

        public async Task<string> GenerateSetupCode(string userId, string email)
        {
            var userMfa = await _context.UserMfaSecrets
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userMfa == null)
            {
                var secretKey = KeyGeneration.GenerateRandomKey(20);
                userMfa = new UserMfaSecret
                {
                    UserId = userId,
                    SecretKey = secretKey,
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserMfaSecrets.Add(userMfa);
                await _context.SaveChangesAsync();
            }

            // Format secret key as Base32 for QR code generation
            var base32Secret = Base32Encoding.ToString(userMfa.SecretKey);
            var issuer = "SecureWebApp";

            // Standard format for authenticator apps
            var setupCode = $"otpauth://totp/{issuer}:{email}?secret={base32Secret}&issuer={issuer}&digits={Digits}&period={StepSeconds}";

            return setupCode;
        }
    }

    public static class KeyGeneration
    {
        public static byte[] GenerateRandomKey(int length)
        {
            using var rng = RandomNumberGenerator.Create();
            var key = new byte[length];
            rng.GetBytes(key);
            return key;
        }
    }

    public static class Base32Encoding
    {
        private static readonly string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string ToString(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                return string.Empty;
            }

            var bits = input.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Aggregate((a, b) => a + b);
            bits = bits.PadRight((int)(Math.Ceiling(bits.Length / 5.0) * 5), '0');

            var result = Enumerable.Range(0, bits.Length / 5)
                .Select(i => Base32Chars[Convert.ToInt32(bits.Substring(i * 5, 5), 2)])
                .Aggregate("", (s, c) => s + c);

            return result;
        }
    }
}