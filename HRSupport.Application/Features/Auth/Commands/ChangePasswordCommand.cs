using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class ChangePasswordCommand : IRequest<Result<bool>>
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}