using HRSupport.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRSupport.Application.Interfaces
{
    public interface IInternRepository
    {
        Task<IEnumerable<Intern>> GetAllAsync();
        Task<Intern?> GetByIdAsync(int id);
        Task<Intern?> GetByEmailAsync(string email);
        Task<int> AddAsync(Intern intern);
        Task<Intern> UpdateAsync(Intern intern);
        Task<Intern?> DeleteAsync(int id);
    }
}