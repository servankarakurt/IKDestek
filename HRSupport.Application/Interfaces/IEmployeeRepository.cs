using HRSupport.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRSupport.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetByIdAsync(int id);
        Task<int>AddAsync(Employee entites);
        Task<Employee> UpdateAsync(Employee employee);
        Task<Employee> DeleteAsync(int id);

    }
}
