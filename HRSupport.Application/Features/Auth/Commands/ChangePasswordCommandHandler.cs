using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(IUserRepository userRepository, ILogger<ChangePasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("Kullanıcı bulunamadı.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid current password while changing password. UserId: {UserId}", request.UserId);
                return Result<bool>.Failure("Mevcut şifreniz yanlış.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.IsPasswordChangeRequired = false;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Password changed for userId: {UserId}", request.UserId);

            return Result<bool>.Success(true, "Şifreniz başarıyla güncellendi.");
        }
    }
}
