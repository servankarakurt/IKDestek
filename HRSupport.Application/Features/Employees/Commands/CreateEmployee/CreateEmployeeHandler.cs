using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateEmployeeHandler> _logger;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            ILogger<CreateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Employee>(request);

            // Generate a temporary password for the new user and store its hash
            var tempPassword = GenerateTemporaryPassword(10);
            employee.PasswordHash = PasswordHelper.Hash(tempPassword);
            employee.MustChangePassword = true;

            var employeeId = await _employeeRepository.AddAsync(employee);

            _logger.LogInformation("Employee created. EmployeeId: {EmployeeId}, Email: {Email}", employeeId, request.Email);

            // Return the temporary password in the result message so the UI can show it to the admin
            var message = $"Geçici şifre: {tempPassword}. Kullanıcı ilk girişte değiştirmelidir.";
            return Result<int>.Success(employeeId, message);
        }

        private static string GenerateTemporaryPassword(int length = 10)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_";
            var sb = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[4];
            while (sb.Length < length)
            {
                rng.GetBytes(buffer);
                var num = BitConverter.ToUInt32(buffer, 0);
                var idx = (int)(num % (uint)chars.Length);
                sb.Append(chars[idx]);
            }
            return sb.ToString();
        }

    }
}
