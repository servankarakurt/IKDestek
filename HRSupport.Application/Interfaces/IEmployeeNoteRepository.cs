using HRSupport.Domain.Entities;

namespace HRSupport.Application.Interfaces
{
    public interface IEmployeeNoteRepository
    {
        Task<IEnumerable<EmployeeNote>> GetByEmployeeIdAsync(int employeeId);
        Task<EmployeeNote?> GetByIdAsync(int id);
        Task<int> AddAsync(EmployeeNote entity);
    }
}
