using HRSupport.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Log> Logs { get; set; }
        DbSet<WeeklyReportRequest> WeeklyReportRequests { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}