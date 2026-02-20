using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Features.Employees.Commans;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Common;
using HRSupport.Domain.Entites;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateEmployeeHandler> _logger;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<CreateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Employee>(request);
            var employeeId = await _employeeRepository.AddAsync(employee);

            var tempPassword = GenerateTemporaryPassword();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = Roles.Çalışan,
                IsPasswordChangeRequired = true
            };

            await _userRepository.AddAsync(newUser);

            _logger.LogInformation("Employee created. EmployeeId: {EmployeeId}, Email: {Email}", employeeId, request.Email);

            var message = $"Personel başarıyla eklendi. Geçici Şifre: {tempPassword} (İlk girişte şifre değişikliği zorunludur.)";
            return Result<int>.Success(employeeId, message);
        }

        private string GenerateTemporaryPassword()
        {
            var chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
