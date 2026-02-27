using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Infrastructure.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            int? userId,
            string? userType,
            string action,
            string? entityName,
            int? entityId,
            bool success,
            string? message = null,
            string? ipAddress = null,
            string? userAgent = null,
            CancellationToken cancellationToken = default)
        {
            var log = new ActivityLog
            {
                OccurredAtUtc = DateTime.UtcNow,
                UserId = userId,
                UserType = userType ?? null,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Success = success,
                Message = message?.Length > 500 ? message[..500] : message,
                IpAddress = ipAddress?.Length > 50 ? ipAddress[..50] : ipAddress,
                UserAgent = userAgent?.Length > 500 ? userAgent[..500] : userAgent
            };
            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
