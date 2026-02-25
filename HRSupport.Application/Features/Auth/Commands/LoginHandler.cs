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
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInternRepository _internRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginHandler(
            IEmployeeRepository employeeRepository,
            IInternRepository internRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _employeeRepository = employeeRepository;
            _internRepository = internRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return Result<LoginResponseDto>.Failure("E-posta ve şifre zorunludur.");

            var email = request.Email.Trim();

            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee != null)
                return ValidateAndBuildResult(employee, null, request.Password);

            var intern = await _internRepository.GetByEmailAsync(email);
            if (intern != null)
                return ValidateAndBuildResult(null, intern, request.Password);

            return Result<LoginResponseDto>.Failure("Geçersiz e-posta veya şifre.");
        }

        private Result<LoginResponseDto> ValidateAndBuildResult(Employee? employee, Intern? intern, string password)
        {
            if (employee != null)
            {
                if (string.IsNullOrEmpty(employee.PasswordHash))
                    return Result<LoginResponseDto>.Failure("Bu hesap için şifre tanımlanmamış. Lütfen yöneticinize başvurun.");
                if (!PasswordHelper.Verify(password.Trim(), employee.PasswordHash))
                    return Result<LoginResponseDto>.Failure("Geçersiz e-posta veya şifre.");
                var roleName = employee.Roles.ToString();
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
                if (!PasswordHelper.Verify(password.Trim(), intern.PasswordHash))
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
    }
}
