using HRSupport.Domain.Entities;
using System.Threading.Tasks;

namespace HRSupport.Application.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveRequest>> GetAllAsync();
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<int> AddAsync(LeaveRequest leaveRequest);
        Task UpdateAsync(LeaveRequest leaveRequest); // Durum güncellemesi için eklendi
        Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetByEmployeeIdsAsync(IEnumerable<int> employeeIds);
    }
}