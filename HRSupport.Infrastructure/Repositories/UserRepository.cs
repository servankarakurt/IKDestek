using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var normalized = email.Trim().ToLowerInvariant();
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email != null && x.Email.Trim().ToLower() == normalized);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
