using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;

namespace HRSupport.Application.Features.Reports.Queries
{
    public class GetMyReportRequestsHandler : IRequestHandler<GetMyReportRequestsQuery, Result<IEnumerable<WeeklyReportRequest>>>
    {
        private readonly IWeeklyReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public GetMyReportRequestsHandler(
            IWeeklyReportRepository reportRepository,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<IEnumerable<WeeklyReportRequest>>> Handle(GetMyReportRequestsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<IEnumerable<WeeklyReportRequest>>.Failure("Kullanıcı bulunamadı.");
            }

            var employee = await _employeeRepository.GetByEmailAsync(user.Email);
            if (employee == null)
            {
                return Result<IEnumerable<WeeklyReportRequest>>.Success(new List<WeeklyReportRequest>(), "Personel kaydı bulunamadı.");
            }

            var reports = await _reportRepository.GetByEmployeeIdAsync(employee.Id);
            return Result<IEnumerable<WeeklyReportRequest>>.Success(reports ?? new List<WeeklyReportRequest>(), "Rapor talepleri başarıyla getirildi.");
        }
    }
}
