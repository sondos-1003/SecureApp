using System;


using System.ComponentModel.DataAnnotations;

namespace SecureWebApp.backend.Models
{
    public class UserMfaSecret
    {
        [Key]
        public string UserId { get; set; }
        public byte[] SecretKey { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}