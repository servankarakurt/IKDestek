using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;

        public ChangePasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcıyı bul
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("Kullanıcı bulunamadı.");
            }

            // Mevcut şifre doğru mu kontrol et
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Result<bool>.Failure("Mevcut şifreniz yanlış.");
            }

            // Yeni şifreyi Hashle
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Zorunlu değiştirme bayrağını kaldır
            user.IsPasswordChangeRequired = false;

            await _userRepository.UpdateAsync(user);

            return Result<bool>.Success(true, "Şifreniz başarıyla güncellendi.");
        }
    }
}