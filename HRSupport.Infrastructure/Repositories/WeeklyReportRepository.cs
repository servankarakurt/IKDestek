using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Infrastructure.Repositories
{
    public class WeeklyReportRepository : IWeeklyReportRepository
    {
        private readonly ApplicationDbContext _context;

        public WeeklyReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(WeeklyReportRequest reportRequest)
        {
            await _context.WeeklyReportRequests.AddAsync(reportRequest);
            await _context.SaveChangesAsync();
            return reportRequest.Id;
        }

        public async Task<int> AddRangeAsync(IEnumerable<WeeklyReportRequest> reportRequests)
        {
            var reportRequestList = reportRequests.ToList();
            await _context.WeeklyReportRequests.AddRangeAsync(reportRequestList);
            await _context.SaveChangesAsync();
            return reportRequestList.Count;
        }

        public async Task<IEnumerable<WeeklyReportRequest>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.WeeklyReportRequests
                .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }
    }
}
