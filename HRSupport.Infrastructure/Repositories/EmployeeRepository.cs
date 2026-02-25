using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var normalized = email.Trim().ToLowerInvariant();
            return await _context.Employees.FirstOrDefaultAsync(x => x.Email != null && x.Email.Trim().ToLower() == normalized);
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(Department department)
        {
            return await _context.Employees
                .Where(x => x.Department == department)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Employee entity)
        {
            await _context.Employees.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee?> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee != null)
            {
                employee.IsDeleted = true;
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return employee;
            }

            return null;
        }
    }
}
