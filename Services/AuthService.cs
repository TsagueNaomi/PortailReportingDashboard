using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortailSocadel.Data;
using PortailSocadel.Models;

namespace PortailSocadel.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<User?> AuthenticateOrRegisterUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var cleanEmail = email.Trim().ToLower();
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == cleanEmail);

            if (existingUser != null)
            {
                if (!VerifyPassword(password, existingUser.PasswordHash))
                    return null;

                return existingUser;
            }

            // Automatiquement inscrire le nouvel utilisateur en base de données avec le rôle "User"
            string derivedName = cleanEmail.Split('@')[0];
            if (derivedName.Contains('.'))
            {
                var parts = derivedName.Split('.');
                derivedName = string.Join(" ", parts.Select(p => p.Length > 0 ? char.ToUpper(p[0]) + p[1..] : p));
            }
            else if (derivedName.Length > 0)
            {
                derivedName = char.ToUpper(derivedName[0]) + derivedName[1..];
            }

            var newUser = new User
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Email = cleanEmail,
                FullName = derivedName,
                PasswordHash = HashPassword(password),
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            DbSeeder.SaveData(_context);
            return newUser;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());
        }

        public async Task<User> RegisterUserAsync(string email, string fullName, string password, string role = "User")
        {
            var existing = await GetUserByEmailAsync(email);
            if (existing != null)
                throw new InvalidOperationException("Un utilisateur avec cet email existe déjà.");

            var user = new User
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Email = email.Trim().ToLower(),
                FullName = fullName.Trim(),
                PasswordHash = HashPassword(password),
                Role = role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            DbSeeder.SaveData(_context);
            return user;
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return string.Equals(hashOfInput, hashedPassword);
        }
    }
}
