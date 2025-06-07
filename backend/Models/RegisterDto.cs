using System.ComponentModel.DataAnnotations;

namespace SecureWebApp.Models
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required] public string Email {  get; set; } = string.Empty;
        [Required] public string Role { get; set; } = "User";
    }
}
