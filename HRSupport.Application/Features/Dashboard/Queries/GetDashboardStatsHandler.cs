using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Domain.Enum;
using MediatR;
using System.Linq; // .Where ve .Select kullanabilmek için gerekli
using System.Threading;
using System.Threading.Tasks;

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

            // İstatistikleri hesaplıyoruz ve listeyi DTO'ya bağlıyoruz (Hepsi tek bir blokta)
            var stats = new DashboardStatsDto
            {
                TotalEmployees = employees.Count(),
                TotalInterns = interns.Count(),
                PendingLeaveRequests = leaveRequests.Count(x => x.Status == LeaveStatus.Beklemede),
                ApprovedLeaveRequests = leaveRequests.Count(x => x.Status == LeaveStatus.Onaylandı),

                // Bekleyen izinleri listeye alıyoruz
                RecentPendingRequests = leaveRequests
                    .Where(x => x.Status == LeaveStatus.Beklemede)
                    .Select(x => new LeaveRequestDto
                    {
                        Id = x.Id,
                        EmployeeId = x.EmployeeId,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Type = x.Type,
                        IsUrgentWithoutBalance = x.IsUrgentWithoutBalance
                    })
                    .ToList()
            };

            // Metodun en sonundaki dönüş işlemi
            return Result<DashboardStatsDto>.Success(stats);
        }
    }
}