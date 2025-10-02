using Microsoft.Extensions.Caching.Memory;
namespace StudentClub.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);

        string GenerateCode(int length);
        void SaveResetCode(IMemoryCache cache, string email, string code, TimeSpan expiration);
        bool VerifyResetCode(IMemoryCache cache, string email, string code);
        void RemoveResetCode(IMemoryCache cache, string email);
    }
}
