using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Reports.Commands
{
    public class CreateReportRequestHandler : IRequestHandler<CreateReportRequestCommand, Result<int>>
    {
        private readonly IWeeklyReportRepository _weeklyReportRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<CreateReportRequestHandler> _logger;

        public CreateReportRequestHandler(
            IWeeklyReportRepository weeklyReportRepository,
            IEmployeeRepository employeeRepository,
            ILogger<CreateReportRequestHandler> logger)
        {
            _weeklyReportRepository = weeklyReportRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateReportRequestCommand request, CancellationToken cancellationToken)
        {
            var managerEmployee = await _employeeRepository.GetByIdAsync(request.ManagerEmployeeId);
            if (managerEmployee == null)
            {
                return Result<int>.Failure("Yönetici personel kaydı bulunamadı.");
            }

            var sameDepartmentEmployees = await _employeeRepository.GetByDepartmentAsync(managerEmployee.Department);
            var targetEmployees = sameDepartmentEmployees.Where(x => x.Id != managerEmployee.Id).ToList();

            if (!targetEmployees.Any())
            {
                return Result<int>.Failure("Aynı birimde rapor talebi gönderilecek personel bulunamadı.");
            }

            var reportRequests = targetEmployees.Select(employee => new WeeklyReportRequest
            {
                ManagerId = managerEmployee.Id,
                EmployeeId = employee.Id,
                Description = request.Description,
                DueDate = request.DueDate
            }).ToList();

            var createdCount = await _weeklyReportRepository.AddRangeAsync(reportRequests);

            _logger.LogInformation(
                "ManagerEmployeeId {ManagerEmployeeId} tarafından Department {Department} için {Count} kişiye haftalık rapor talebi gönderildi.",
                request.ManagerEmployeeId,
                managerEmployee.Department,
                createdCount);

            return Result<int>.Success(createdCount, $"{createdCount} personele haftalık rapor bildirimi oluşturuldu.");
        }
    }
}
