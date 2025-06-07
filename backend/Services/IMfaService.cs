using OtpNet;
namespace SecureWebApp.backend.Services
{
    public interface IMfaService
    {
        Task<string> GenerateCode(string userId);
        Task<bool> VerifyCode(string userId, string code);
        Task<string> GenerateSetupCode(string userId, string email);
    }
}
