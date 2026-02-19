using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Enum;
using MediatR;

namespace HRSupport.Application.Features.Dashboard.Queries
{
    public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInternRepository _internRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public GetDashboardStatsHandler(
            IEmployeeRepository employeeRepository,
            IInternRepository internRepository,
            ILeaveRequestRepository leaveRequestRepository)
        {
            _employeeRepository = employeeRepository;
            _internRepository = internRepository;
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            // Veritabanından verileri çekiyoruz
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            var interns = await _internRepository.GetAllAsync();
            var leaveRequests = await _leaveRequestRepository.GetAllAsync();

            // İstatistikleri hesaplıyoruz
            var stats = new DashboardStatsDto
            {
                TotalEmployees = employees.Count(),
                TotalInterns = interns.Count(),
                PendingLeaveRequests = leaveRequests.Count(x => x.Status == LeaveStatus.Beklemede),
                ApprovedLeaveRequests = leaveRequests.Count(x => x.Status == LeaveStatus.Onaylandı)
            };

            return Result<DashboardStatsDto>.Success(stats);
        }
    }
}