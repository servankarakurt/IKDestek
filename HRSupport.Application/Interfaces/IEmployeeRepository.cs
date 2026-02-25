using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;

namespace HRSupport.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByEmailAsync(string email);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(Department department);
        Task<int> AddAsync(Employee entity);
        Task<Employee> UpdateAsync(Employee employee);
        Task<Employee?> DeleteAsync(int id);
    }
}
