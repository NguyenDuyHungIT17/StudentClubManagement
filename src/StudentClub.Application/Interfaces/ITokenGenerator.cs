namespace StudentClub.Application.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(int userId, string email, string role);
    }
}
