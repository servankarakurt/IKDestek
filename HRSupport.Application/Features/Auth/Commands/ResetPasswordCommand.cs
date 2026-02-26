using HRSupport.Application.Common;
using MediatR;

namespace HRSupport.Application.Features.Auth.Commands
{
    /// <summary>Admin/IK: E-posta ile kullanıcı/çalışan/stajyer şifresini geçici şifre ile sıfırlar. Yeni geçici şifre döner.</summary>
    public class ResetPasswordCommand : IRequest<Result<string>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
