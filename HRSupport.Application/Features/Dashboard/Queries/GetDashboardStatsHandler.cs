using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Dashboard.Queries
{
    public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInternRepository _internRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ICurrentUserService _currentUser;

        public GetDashboardStatsHandler(
            IEmployeeRepository employeeRepository,
            IInternRepository internRepository,
            ILeaveRequestRepository leaveRequestRepository,
            ICurrentUserService currentUser)
        {
            _employeeRepository = employeeRepository;
            _internRepository = internRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var role = _currentUser.Role ?? string.Empty;
            var departmentId = _currentUser.DepartmentId;
            var isManager = role == "Yönetici";

            List<Employee> employeesList;
            IEnumerable<LeaveRequest> pendingLeaves;
            List<DepartmentCountDto> departmentBreakdown;

            if (isManager && departmentId.HasValue)
            {
                // Yönetici: sadece kendi birimindeki çalışanlar ve onların bekleyen izinleri
                var dept = (Department)departmentId.Value;
                var deptEmployees = (await _employeeRepository.GetByDepartmentAsync(dept)).ToList();
                employeesList = deptEmployees.ToList();
                var employeeIds = employeesList.Select(e => e.Id).ToList();
                var allLeavesForDept = await _leaveRequestRepository.GetByEmployeeIdsAsync(employeeIds);
                pendingLeaves = allLeavesForDept.Where(x => x.Status == LeaveStatus.Beklemede).ToList();

                departmentBreakdown = new List<DepartmentCountDto>
                {
                    new DepartmentCountDto
                    {
                        DepartmentId = (int)dept,
                        DepartmentName = GetDepartmentDisplayName(dept),
                        Count = employeesList.Count
                    }
                };
            }
            else
            {
                // Admin / IK: tüm çalışanlar, tüm bekleyen izinler, tüm departmanlar
                employeesList = (await _employeeRepository.GetAllEmployeesAsync()).ToList();
                var allLeaveRequests = await _leaveRequestRepository.GetAllAsync();
                pendingLeaves = allLeaveRequests.Where(x => x.Status == LeaveStatus.Beklemede).ToList();

                departmentBreakdown = employeesList
                    .GroupBy(e => e.Department)
                    .OrderBy(g => g.Key)
                    .Select(g => new DepartmentCountDto
                    {
                        DepartmentId = (int)g.Key,
                        DepartmentName = GetDepartmentDisplayName(g.Key),
                        Count = g.Count()
                    })
                    .ToList();
            }

            var interns = await _internRepository.GetAllAsync();
            var allLeaveRequestsForApproved = await _leaveRequestRepository.GetAllAsync();

            var employeeDict = employeesList.ToDictionary(e => e.Id, e => e.FullName);
            var recentPending = pendingLeaves
                .Select(x => new LeaveRequestDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    EmployeeName = employeeDict.TryGetValue(x.EmployeeId, out var name) ? name : null,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Type = x.Type,
                    IsUrgentWithoutBalance = x.IsUrgentWithoutBalance
                })
                .ToList();

            var stats = new DashboardStatsDto
            {
                TotalEmployees = employeesList.Count,
                TotalInterns = isManager ? 0 : interns.Count(),
                PendingLeaveRequests = recentPending.Count,
                ApprovedLeaveRequests = isManager ? 0 : allLeaveRequestsForApproved.Count(x => x.Status == LeaveStatus.Onaylandı),
                RecentPendingRequests = recentPending,
                DepartmentBreakdown = departmentBreakdown,
                IsManagerView = isManager,
                ManagerDepartmentName = isManager && departmentId.HasValue ? GetDepartmentDisplayName((Department)departmentId.Value) : null
            };

            return Result<DashboardStatsDto>.Success(stats);
        }

        private static string GetDepartmentDisplayName(Department dept)
        {
            return dept switch
            {
                Department.Yazilim => "Yazılım",
                Department.InsanKaynaklari => "İnsan Kaynakları",
                Department.Satis => "Satış",
                Department.Muhasebe => "Muhasebe",
                Department.Pazarlama => "Pazarlama",
                Department.Operasyon => "Operasyon",
                Department.Acente => "Acente",
                _ => dept.ToString()
            };
        }
    }
}