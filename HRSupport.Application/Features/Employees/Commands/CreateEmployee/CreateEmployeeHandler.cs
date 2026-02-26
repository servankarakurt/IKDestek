using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeLeaveBalanceRepository _balanceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateEmployeeHandler> _logger;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeLeaveBalanceRepository balanceRepository,
            IMapper mapper,
            ILogger<CreateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _balanceRepository = balanceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Employee>(request);

            // E-posta tutarlı arama için küçük harf ve trim (girişte aynı normalleştirme kullanılıyor)
            if (!string.IsNullOrWhiteSpace(employee.Email))
                employee.Email = employee.Email.Trim().ToLowerInvariant();

            // Geçici şifre: sadece alfanumerik (kopyala-yapıştır ve HTML kaçış sorunlarını önlemek için)
            var tempPassword = PasswordHelper.GenerateTemporaryPassword(10);
            employee.PasswordHash = PasswordHelper.Hash(tempPassword);
            employee.MustChangePassword = true;

            var employeeId = await _employeeRepository.AddAsync(employee);

            await _balanceRepository.AddAsync(new EmployeeLeaveBalance
            {
                EmployeeId = employeeId,
                RemainingAnnualLeaveDays = 20
            });

            _logger.LogInformation("Employee created. EmployeeId: {EmployeeId}, Email: {Email}", employeeId, request.Email);

            // Return the temporary password in the result message so the UI can show it to the admin
            var message = $"Geçici şifre: {tempPassword}. Kullanıcı ilk girişte değiştirmelidir.";
            return Result<int>.Success(employeeId, message);
        }

    }
}
