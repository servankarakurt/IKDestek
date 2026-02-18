using HRSupport.Domain.Entites;
using System.Threading.Tasks;

namespace HRSupport.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<int> AddAsync(User user);
    }
}
