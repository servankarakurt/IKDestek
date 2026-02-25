using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRSupport.Infrastructure.Repositories
{
    public class EmployeeLeaveBalanceRepository : IEmployeeLeaveBalanceRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeLeaveBalanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeLeaveBalance?> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeLeaveBalances
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
        }
    }
}
