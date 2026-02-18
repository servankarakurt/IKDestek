using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Infrastructure.Context;
using System.Threading.Tasks;

namespace HRSupport.Infrastructure.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(LeaveRequest entity)
        {
            await _context.LeaveRequests.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
    }
}