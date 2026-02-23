using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogRepository _context; // Veya ILogRepository

        public LoggingBehavior(ILogRepository context)
        {
            _context = context;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Sadece Command'leri (Ekleme, Silme, Güncelleme) loglamak istiyorsak:
            if (request.GetType().Name.EndsWith("Command"))
            {
                var log = new Log
                {
                    ActionName = request.GetType().Name,
                    Details = JsonSerializer.Serialize(request),
                    Timestamp = DateTime.Now,
                    // UserId bilgisini bir ICurrentUserService üzerinden alabilirsin
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
            }

            return await next();
        }
    }
}