using System.Security.Cryptography;
using System.Text;

namespace SecureWebApp.Helpers
{
    public static class AesEncryptionHelper
    {
        private static readonly string key = "mysupersecretkey1234567890123456"; // 32 bytes
        private static readonly string iv = "1234567890123456"; // 16 bytes

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv); // ✅ 16-byte IV

            var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(cipherBytes);
        }

        public static string Decrypt(string encrypted)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv); // ✅ match above

            var cipherText = Convert.FromBase64String(encrypted);
            var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }


    }
}
