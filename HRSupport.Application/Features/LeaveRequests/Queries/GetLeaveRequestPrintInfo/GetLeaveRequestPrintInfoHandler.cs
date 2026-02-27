using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Queries.GetLeaveRequestPrintInfo
{
    public class GetLeaveRequestPrintInfoHandler : IRequestHandler<GetLeaveRequestPrintInfoQuery, Result<LeaveRequestPrintInfoDto>>
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInternRepository _internRepository;
        private readonly ICurrentUserService _currentUser;

        public GetLeaveRequestPrintInfoHandler(
            ILeaveRequestRepository leaveRepository,
            IEmployeeRepository employeeRepository,
            IInternRepository internRepository,
            ICurrentUserService currentUser)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _internRepository = internRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<LeaveRequestPrintInfoDto>> Handle(GetLeaveRequestPrintInfoQuery request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null)
                return Result<LeaveRequestPrintInfoDto>.Failure("İzin talebi bulunamadı.");

            var role = _currentUser.Role ?? string.Empty;
            var userId = _currentUser.UserId;
            var departmentId = _currentUser.DepartmentId;

            if (role == "Admin" || role == "IK") { }
            else if (role == "Yönetici")
            {
                var empCheck = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
                if (empCheck == null || !departmentId.HasValue || (int)empCheck.Department != departmentId.Value)
                    return Result<LeaveRequestPrintInfoDto>.Failure("Bu izin talebini görüntüleme yetkiniz yok.");
            }
            else if (role == "Çalışan" || role == "Stajyer")
            {
                if (!userId.HasValue || leaveRequest.EmployeeId != userId.Value)
                    return Result<LeaveRequestPrintInfoDto>.Failure("Sadece kendi izin talebinizi yazdırabilirsiniz.");
            }
            else
                return Result<LeaveRequestPrintInfoDto>.Failure("Bu işlem için yetkiniz yok.");

            var days = leaveRequest.RequestedDays;
            var tr = CultureInfo.GetCultureInfo("tr-TR");

            var employee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
            if (employee != null)
            {
                var dto = new LeaveRequestPrintInfoDto
                {
                    FormPrintDate = DateTime.Now,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}".Trim(),
                    Kurum = "Hepiyi Sigorta",
                    Sirket = "Hepiyi Sigorta A.Ş.",
                    DepartmentName = GetDepartmentName((int)employee.Department),
                    Unvan = "",
                    SicilNo = employee.CardID,
                    LeaveYear = leaveRequest.StartDate.Year,
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate,
                    RequestedDays = days,
                    AddressAndPhone = leaveRequest.Description ?? "",
                    EmployeeStartDate = employee.StartDate,
                    KullanilanIzinGunuDisplay = days.ToString(tr)
                };
                return Result<LeaveRequestPrintInfoDto>.Success(dto);
            }

            var intern = await _internRepository.GetByIdAsync(leaveRequest.EmployeeId);
            if (intern != null)
            {
                var dto = new LeaveRequestPrintInfoDto
                {
                    FormPrintDate = DateTime.Now,
                    EmployeeName = (intern.FullName ?? $"{intern.FirstName} {intern.LastName}").Trim(),
                    Kurum = "Hepiyi Sigorta",
                    Sirket = "Hepiyi Sigorta A.Ş.",
                    DepartmentName = "Stajyer",
                    Unvan = "",
                    SicilNo = 0,
                    LeaveYear = leaveRequest.StartDate.Year,
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate,
                    RequestedDays = days,
                    AddressAndPhone = leaveRequest.Description ?? "",
                    EmployeeStartDate = intern.StartDate,
                    KullanilanIzinGunuDisplay = days.ToString(tr)
                };
                return Result<LeaveRequestPrintInfoDto>.Success(dto);
            }

            return Result<LeaveRequestPrintInfoDto>.Failure("Çalışan veya stajyer bilgisi bulunamadı.");
        }

        private static string GetDepartmentName(int department)
        {
            return department switch
            {
                1 => "Yazılım",
                2 => "İnsan Kaynakları",
                3 => "Satış",
                4 => "Muhasebe",
                5 => "Pazarlama",
                6 => "Operasyon",
                7 => "Acente",
                _ => ""
            };
        }
    }
}
