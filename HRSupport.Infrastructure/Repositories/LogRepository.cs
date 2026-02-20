using HRSupport.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace HRSupport.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly ILogger<LogRepository> _logger;

        public LogRepository(ILogger<LogRepository> logger)
        {
            _logger = logger;
        }

        public Task LogInfoAsync(string message)
        {
            _logger.LogInformation(message);
            return Task.CompletedTask;
        }

        public Task LogWarningAsync(string message)
        {
            _logger.LogWarning(message);
            return Task.CompletedTask;
        }

        public Task LogErrorAsync(string message)
        {
            _logger.LogError(message);
            return Task.CompletedTask;
        }
    }
}
