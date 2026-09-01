using System.Threading.Tasks;
using PortailSocadel.Models;

namespace PortailSocadel.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> RegisterUserAsync(string email, string fullName, string password, string role = "User");
        Task<User?> AuthenticateOrRegisterUserAsync(string email, string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
