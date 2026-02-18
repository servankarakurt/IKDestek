using Dapper;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRSupport.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DapperContext _context;

        public EmployeeRepository(DapperContext context)
        {
            _context = context;
        }

        // DÜZELTME: Interface'deki isim 'GetAllEmployeesAsync' olduğu için burayı değiştirdik.
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var query = "SELECT * FROM Employees WHERE isDelete = 0";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Employee>(query);
            }
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Employees WHERE Id = @Id AND isDelete = 0";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Employee>(query, new { Id = id });
            }
        }

        public async Task<int> AddAsync(Employee entity)
        {
            var query = @"INSERT INTO Employees (FirstName, LastName, Email, Phone, CardID, BirthDate, StartDate, Department, Roles, CreatedDate, isActive, isDeleted)
                          VALUES (@FirstName, @LastName, @Email, @Phone, @CardID, @BirthDate, @StartDate, @Department, @Roles, @CreatedDate, @isActive, @isDeleted);
                          SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(query, entity);
            }
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            var query = @"UPDATE Employees 
                          SET FirstName = @FirstName,
                              LastName = @LastName,
                              Email = @Email,
                              Phone = @Phone,
                              CardID = @CardID,
                              BirthDate = @BirthDate,
                              StartDate = @StartDate,
                              Department = @Department,
                              Roles = @Roles 
                          WHERE Id = @Id AND isDelete = 0";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, employee);
                return employee;
            }
        }

        public async Task<Employee> DeleteAsync(int id)
        {
            var query = "UPDATE Employees SET isDelete = 1 WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return null;
            }
        }
    }
}