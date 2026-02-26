using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class ChangePasswordCommand : IRequest<Result<bool>>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
