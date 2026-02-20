using HRSupport.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Employee> Employees { get; }
        DbSet<Intern> Interns { get; }
        DbSet<User> Users { get; }
        DbSet<LeaveRequest> LeaveRequests { get; }
        DbSet<WeeklyReportRequest> WeeklyReportRequests { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
