using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<Result<string>> 
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}