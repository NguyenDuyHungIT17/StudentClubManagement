using Microsoft.Extensions.Caching.Memory;
using StudentClub.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace StudentClub.Infrastructure.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GenerateCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public string Hash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower(); 
        }

        public bool Verify(string password, string storedHash)
        {
            var hashed = Hash(password);
            return hashed == storedHash.ToLower();
        }
        public void SaveResetCode(IMemoryCache cache, string email, string code, TimeSpan expiration)
        {
            var cacheKey = $"PasswordReset_{email}";
            cache.Set(cacheKey, code, expiration);
        }

        public bool VerifyResetCode(IMemoryCache cache, string email, string code)
        {
            var cacheKey = $"PasswordReset_{email}";
            if (cache.TryGetValue(cacheKey, out string? storedCode) && storedCode == code)
            {
                cache.Remove(cacheKey);
                return true;
            }
            return false;
        }
        public void RemoveResetCode(IMemoryCache cache, string email)
        {
            var cacheKey = $"resetpwd:{email.ToLower()}";
            cache.Remove(cacheKey);
        }
    }
}
