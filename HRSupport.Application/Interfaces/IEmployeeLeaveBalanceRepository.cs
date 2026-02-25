using HRSupport.Domain.Entities;
using System.Threading.Tasks;

namespace HRSupport.Application.Interfaces
{
    public interface IEmployeeLeaveBalanceRepository
    {
        Task<EmployeeLeaveBalance?> GetByEmployeeIdAsync(int employeeId);
    }
}
