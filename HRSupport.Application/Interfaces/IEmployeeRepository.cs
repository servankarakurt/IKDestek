using HRSupport.Domain.Entites;

namespace HRSupport.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetByIdAsync(int id);
        Task<int> AddAsync(Employee entites);
        Task<Employee> UpdateAsync(Employee employee);
        Task<Employee> DeleteAsync(int id);

    }
}
