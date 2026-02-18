using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            // DbContext içindeki HasQueryFilter sayesinde silinmiş olanlar otomatik elenecek
            return await _context.Employeess.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employeess.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(Employee entity)
        {
            await _context.Employeess.AddAsync(entity);
            await _context.SaveChangesAsync();

            // EF Core ekleme işleminden sonra Identity(Id) değerini otomatik olarak entity'e atar
            return entity.Id;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _context.Employeess.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> DeleteAsync(int id)
        {
            // Soft delete işlemi
            var employee = await _context.Employeess.FirstOrDefaultAsync(x => x.Id == id);
            if (employee != null)
            {
                employee.IsDeleted = true; // Sadece bayrağı güncelliyoruz
                _context.Employeess.Update(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            return null;
        }
    }
}