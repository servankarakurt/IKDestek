namespace HRSupport.Application.Interfaces
{
    public interface ILogRepository
    {
        Task LogInfoAsync(string message);
        Task LogWarningAsync(string message);
        Task LogErrorAsync(string message);
    }
}
