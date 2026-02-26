using HRSupport.Domain.Entities;

namespace HRSupport.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(User user);
    }
}
