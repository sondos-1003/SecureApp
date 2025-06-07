using SecureWebApp.Models;

namespace SecureWebApp.Services
{
    public interface IAuthService
    {
        Task<AuthResult> Register(RegisterDto dto);
        Task<AuthResult> Login(LoginDto dto);
    }
}