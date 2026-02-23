using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;

namespace HRSupport.Application.Features.Reports.Queries
{
    public class GetMyReportRequestsHandler : IRequestHandler<GetMyReportRequestsQuery, Result<IEnumerable<WeeklyReportRequest>>>
    {
        private readonly IWeeklyReportRepository _reportRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public GetMyReportRequestsHandler(
            IWeeklyReportRepository reportRepository,
            IEmployeeRepository employeeRepository)
        {
            _reportRepository = reportRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<IEnumerable<WeeklyReportRequest>>> Handle(GetMyReportRequestsQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                return Result<IEnumerable<WeeklyReportRequest>>.Failure("Personel bulunamadı.");
            }

            var reports = await _reportRepository.GetByEmployeeIdAsync(employee.Id);
            return Result<IEnumerable<WeeklyReportRequest>>.Success(reports ?? new List<WeeklyReportRequest>(), "Rapor talepleri başarıyla getirildi.");
        }
    }
}
