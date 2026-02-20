using HRSupport.Domain.Entites;

namespace HRSupport.Application.Interfaces
{
    public interface IWeeklyReportRepository
    {
        Task<int> AddAsync(WeeklyReportRequest reportRequest);
        Task<int> AddRangeAsync(IEnumerable<WeeklyReportRequest> reportRequests);
        Task<IEnumerable<WeeklyReportRequest>> GetByEmployeeIdAsync(int employeeId);
    }
}
