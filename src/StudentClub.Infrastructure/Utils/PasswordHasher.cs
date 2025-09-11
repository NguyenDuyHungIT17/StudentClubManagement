using StudentClub.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace StudentClub.Infrastructure.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
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
    }
}
