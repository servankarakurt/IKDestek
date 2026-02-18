using HRSupport.Domain.Entites;
using System.Threading.Tasks;

namespace HRSupport.Application.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<int> AddAsync(LeaveRequest entity);
    }
}