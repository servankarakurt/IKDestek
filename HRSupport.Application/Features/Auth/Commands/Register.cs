using HRSupport.Application.Common;
using HRSupport.Domain.Common;
using HRSupport.Domain.Enum;
using MediatR;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<Result<int>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Roles Role { get; set; }
    }
}