using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInternRepository _internRepository;

        public ResetPasswordHandler(
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

        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var role = _currentUser.Role ?? "";
            if (role != "Admin" && role != "IK")
                return Result<string>.Failure("Bu işlem sadece Admin veya İK tarafından yapılabilir.");

            var email = (request.Email ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(email))
                return Result<string>.Failure("E-posta adresi gerekli.");

            var tempPassword = PasswordHelper.GenerateTemporaryPassword(10);

            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
            {
                user.PasswordHash = PasswordHelper.Hash(tempPassword);
                user.IsPasswordChangeRequired = true;
                await _userRepository.UpdateAsync(user);
                return Result<string>.Success(tempPassword, "Şifre sıfırlandı. Kullanıcı ilk girişte değiştirmelidir.");
            }

            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee != null)
            {
                employee.PasswordHash = PasswordHelper.Hash(tempPassword);
                employee.MustChangePassword = true;
                await _employeeRepository.UpdateAsync(employee);
                return Result<string>.Success(tempPassword, "Şifre sıfırlandı. Çalışan ilk girişte değiştirmelidir.");
            }

            var intern = await _internRepository.GetByEmailAsync(email);
            if (intern != null)
            {
                intern.PasswordHash = PasswordHelper.Hash(tempPassword);
                intern.MustChangePassword = true;
                await _internRepository.UpdateAsync(intern);
                return Result<string>.Success(tempPassword, "Şifre sıfırlandı. Stajyer ilk girişte değiştirmelidir.");
            }

            return Result<string>.Failure("Bu e-posta adresiyle kayıtlı kullanıcı bulunamadı.");
        }
    }
}
