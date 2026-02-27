using HRSupport.Application.Interfaces;
using MediatR;

namespace HRSupport.Application.Common
{
    /// <summary>
    /// Command işlemlerini (Create/Update/Delete vb.) ActivityLog'a yazar. Auth command'ları (Login, ChangePassword, ResetPassword) atlanır.
    /// </summary>
    public class ActivityLogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private static readonly HashSet<string> SkipCommands = new(StringComparer.OrdinalIgnoreCase)
        {
            "LoginCommand", "ChangePasswordCommand", "ResetPasswordCommand"
        };

        private readonly IActivityLogService _activityLog;
        private readonly ICurrentUserService _currentUser;

        public ActivityLogBehavior(IActivityLogService activityLog, ICurrentUserService currentUser)
        {
            _activityLog = activityLog;
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            var typeName = request?.GetType().Name ?? "";
            if (!typeName.EndsWith("Command", StringComparison.OrdinalIgnoreCase) || SkipCommands.Contains(typeName))
                return response;

            var action = typeName.EndsWith("Command") ? typeName[..^7] : typeName; // "CreateLeaveRequest"
            var entityName = DeriveEntityName(action);

            var success = true;
            var entityId = (int?)null;

            if (response is IResult result)
                success = result.IsSuccess;

            if (success && response != null)
            {
                var responseType = response.GetType();
                if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var valueProp = responseType.GetProperty("Value");
                    if (valueProp?.GetValue(response) is int id)
                        entityId = id;
                }
            }

            await _activityLog.LogAsync(
                _currentUser.UserId,
                _currentUser.UserType,
                action,
                entityName,
                entityId,
                success,
                null,
                null,
                null,
                cancellationToken);

            return response;
        }

        private static string? DeriveEntityName(string action)
        {
            if (string.IsNullOrEmpty(action)) return null;
            var prefixes = new[] { "Create", "Update", "Delete", "Add" };
            foreach (var prefix in prefixes)
            {
                if (action.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && action.Length > prefix.Length)
                    return action[prefix.Length..];
            }
            return null;
        }
    }
}
