using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInternRepository _internRepository;

        public ChangePasswordHandler(
            ICurrentUserService currentUser,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IInternRepository internRepository)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _internRepository = internRepository;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.UserId.HasValue)
                return Result<bool>.Failure("Oturum bulunamadı. Lütfen tekrar giriş yapın.");

            var currentPassword = request.CurrentPassword?.Trim() ?? "";
            var newPassword = request.NewPassword?.Trim() ?? "";

            if (string.IsNullOrEmpty(currentPassword))
                return Result<bool>.Failure("Mevcut şifre gereklidir.");
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                return Result<bool>.Failure("Yeni şifre en az 6 karakter olmalıdır.");

            var userId = _currentUser.UserId.Value;
            var userType = _currentUser.UserType ?? "";

            if (userType == "User")
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return Result<bool>.Failure("Kullanıcı bulunamadı.");
                if (!PasswordHelper.Verify(currentPassword, user.PasswordHash))
                    return Result<bool>.Failure("Mevcut şifre hatalı.");
                user.PasswordHash = PasswordHelper.Hash(newPassword);
                user.IsPasswordChangeRequired = false;
                await _userRepository.UpdateAsync(user);
                return Result<bool>.Success(true, "Şifreniz güncellendi.");
            }

            if (userType == "Employee")
            {
                var employee = await _employeeRepository.GetByIdAsync(userId);
                if (employee == null)
                    return Result<bool>.Failure("Kullanıcı bulunamadı.");
                if (!PasswordHelper.Verify(currentPassword, employee.PasswordHash))
                    return Result<bool>.Failure("Mevcut şifre hatalı.");
                employee.PasswordHash = PasswordHelper.Hash(newPassword);
                employee.MustChangePassword = false;
                await _employeeRepository.UpdateAsync(employee);
                return Result<bool>.Success(true, "Şifreniz güncellendi.");
            }

            if (userType == "Intern")
            {
                var intern = await _internRepository.GetByIdAsync(userId);
                if (intern == null)
                    return Result<bool>.Failure("Kullanıcı bulunamadı.");
                if (!PasswordHelper.Verify(currentPassword, intern.PasswordHash))
                    return Result<bool>.Failure("Mevcut şifre hatalı.");
                intern.PasswordHash = PasswordHelper.Hash(newPassword);
                intern.MustChangePassword = false;
                await _internRepository.UpdateAsync(intern);
                return Result<bool>.Success(true, "Şifreniz güncellendi.");
            }

            return Result<bool>.Failure("Geçersiz kullanıcı türü.");
        }
    }
}
