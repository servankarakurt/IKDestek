using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Infrastructure.Context;
using System.Threading.Tasks;

namespace HRSupport.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DapperContext _context;
        EmployeeRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var query = "SELECT * FROM Employees WHERE isDelete==0";
            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);
                return await connection.QueryAsync<Employee>(query);
            }
        }
        public async Task<Employee> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Employees WHERE Id = @Id AND isDelete==0";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Employee>(query, new { Id = id });
            }
        }
        public async Task<int> AddAsync(Employee entity)
        {
            var query = @"INSERT INTO Employees(FirsName,LastName,Email,Phone,CardID,BirthDate,StartDate,Department,Roles,CreatedDate,isActive,isDeleted )" +
                "Values (@FirsName,@LastName,@Email,@Phone,@CardID,@BirthDate,@StartDate,@Department,@Roles,@CreatedDate,@isActive,@isDeleted);
                 SELECT CAST(SCOPE_IDENTITY() as int)";
                 using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(query, entity);
            }
        }
        public async Task<Employee> UpdateAsync(Employee employee)
        {
            var query = @"UPDATE Employees SET FirsName = @FirsName,LastName = @LastName,Email = @Email,Phone = @Phone,CardID = @CardID,BirthDate = @BirthDate,StartDate = @StartDate,Department = @Department,Roles = @Roles WHERE Id = @Id AND isDelete==0";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, employee);
                return employee;
            }
        }
        public async Task<Employee> DeleteAsync(int id)
        {
            var query = @"UPDATE Employees SET isDelete = 1 WHERE Id = @Id AND isDelete==0";
            using (var connection = _context.CreateConnection()) 
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return await GetByIdAsync(id);
            }
        }
    } 
}

