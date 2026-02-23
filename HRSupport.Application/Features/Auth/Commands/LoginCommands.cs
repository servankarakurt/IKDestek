using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Auth.Commands
{
    // Buradaki <Result<string>> kısmını <Result<object>> olarak değiştirdik
    public class LoginCommand : IRequest<Result<object>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}