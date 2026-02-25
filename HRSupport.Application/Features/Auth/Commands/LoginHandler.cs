using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInternRepository _internRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginHandler(
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IInternRepository internRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _internRepository = internRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return Result<LoginResponseDto>.Failure("E-posta ve şifre zorunludur.");

            // Personel/Stajyer ile aynı normalleştirme (kayıtta Trim + ToLowerInvariant kullanılıyor)
            var email = request.Email.Trim().ToLowerInvariant();
            var password = request.Password.Trim();

            // 1) Önce Users tablosunda ara (migration ile seed edilen Admin/IK: admin@hepiyi.com vb.)
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
                return ValidateAndBuildResultForUser(user, password);

            // 2) Sonra Employees tablosunda ara: Admin, IK, Yönetici, Çalışan (Roles ile ayrılır)
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee != null)
                return ValidateAndBuildResult(employee, null, password);

            // 3) En son Stajyer (Intern) tablosunda ara
            var intern = await _internRepository.GetByEmailAsync(email);
            if (intern != null)
                return ValidateAndBuildResult(null, intern, password);

            return Result<LoginResponseDto>.Failure("Geçersiz e-posta veya şifre.");
        }

        private Result<LoginResponseDto> ValidateAndBuildResultForUser(User user, string password)
        {
            if (string.IsNullOrEmpty(user.PasswordHash))
                return Result<LoginResponseDto>.Failure("Bu hesap için şifre tanımlanmamış. Lütfen yöneticinize başvurun.");
            if (!PasswordHelper.Verify(password, user.PasswordHash))
                return Result<LoginResponseDto>.Failure("Geçersiz e-posta veya şifre.");
            var roleName = NormalizeRole(user.Role);
            var fullName = string.IsNullOrEmpty(user.Email) ? "Sistem Kullanıcısı" : user.Email;
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, roleName, fullName, "User", null);
            return Result<LoginResponseDto>.Success(new LoginResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = roleName,
                UserId = user.Id,
                FullName = fullName,
                UserType = "User",
                MustChangePassword = user.IsPasswordChangeRequired
            });
        }

        private Result<LoginResponseDto> ValidateAndBuildResult(Employee? employee, Intern? intern, string password)
        {
            if (employee != null)
            {
                if (string.IsNullOrEmpty(employee.PasswordHash))
                    return Result<LoginResponseDto>.Failure("Bu hesap için şifre tanımlanmamış. Lütfen yöneticinize başvurun.");
                if (!PasswordHelper.Verify(password, employee.PasswordHash))
                    return Result<LoginResponseDto>.Failure("Geçersiz e-posta veya şifre.");
                var roleName = NormalizeRole(employee.Roles);
                var token = _jwtTokenGenerator.GenerateToken(employee.Id, employee.Email, roleName, employee.FullName, "Employee", (int)employee.Department);
                return Result<LoginResponseDto>.Success(new LoginResponseDto
                {
                    Token = token,
                    Email = employee.Email,
                    Role = roleName,
                    UserId = employee.Id,
                    FullName = employee.FullName,
                    UserType = "Employee",
                    MustChangePassword = employee.MustChangePassword
                });
            }

            if (intern != null)
            {
                if (string.IsNullOrEmpty(intern.PasswordHash))
                    return Result<LoginResponseDto>.Failure("Bu hesap için şifre tanımlanmamış. Lütfen yöneticinize başvurun.");
                if (!PasswordHelper.Verify(password, intern.PasswordHash))
                    return Result<LoginResponseDto>.Failure("Geçersiz e-posta veya şifre.");
                var roleName = Roles.Stajyer.ToString();
                var token = _jwtTokenGenerator.GenerateToken(intern.Id, intern.Email, roleName, intern.FullName, "Intern");
                return Result<LoginResponseDto>.Success(new LoginResponseDto
                {
                    Token = token,
                    Email = intern.Email,
                    Role = roleName,
                    UserId = intern.Id,
                    FullName = intern.FullName,
                    UserType = "Intern",
                    MustChangePassword = intern.MustChangePassword
                });
            }

            return Result<LoginResponseDto>.Failure("Geçersiz e-posta veya şifre.");
        }

        /// <summary>API [Authorize(Roles = "Admin,IK,Yönetici")] ile uyumlu tek tip rol adı döndürür.</summary>
        private static string NormalizeRole(Roles role)
        {
            return role switch
            {
                Roles.Admin => "Admin",
                Roles.IK => "IK",
                Roles.Yönetici => "Yönetici",
                Roles.Çalışan => "Çalışan",
                Roles.Stajyer => "Stajyer",
                _ => role.ToString()
            };
        }
    }
}
