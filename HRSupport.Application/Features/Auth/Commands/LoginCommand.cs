using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<Result<LoginResponseDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
