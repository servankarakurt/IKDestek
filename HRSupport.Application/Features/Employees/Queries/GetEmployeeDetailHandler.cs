using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Enum;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetEmployeeDetailHandler : IRequestHandler<GetEmployeeDetailQuery, Result<EmployeeDetailDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeNoteRepository _noteRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ICurrentUserService _currentUser;

        public GetEmployeeDetailHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeNoteRepository noteRepository,
            ILeaveRequestRepository leaveRequestRepository,
            ICurrentUserService currentUser)
        {
            _employeeRepository = employeeRepository;
            _noteRepository = noteRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<EmployeeDetailDto>> Handle(GetEmployeeDetailQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.Id);
            if (employee == null)
                return Result<EmployeeDetailDto>.Failure("Çalışan bulunamadı.");

            var role = _currentUser.Role ?? "";
            if (role == "Yönetici" && _currentUser.DepartmentId.HasValue && (int)employee.Department != _currentUser.DepartmentId.Value)
                return Result<EmployeeDetailDto>.Failure("Bu çalışanı görüntüleme yetkiniz yok.");
            if (role != "Admin" && role != "IK" && role != "Yönetici")
                return Result<EmployeeDetailDto>.Failure("Bu işlem için yetkiniz yok.");

            var notes = await _noteRepository.GetByEmployeeIdAsync(request.Id);
            var leaveHistory = await _leaveRequestRepository.GetByEmployeeIdAsync(request.Id);

            var dto = new EmployeeDetailDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                Email = employee.Email,
                Phone = employee.Phone,
                TCKN = employee.TCKN,
                CardID = employee.CardID,
                BirthDate = employee.BirthDate,
                StartDate = employee.StartDate,
                Department = (int)employee.Department,
                DepartmentName = GetDepartmentName(employee.Department),
                Roles = (int)employee.Roles,
                RolesName = employee.Roles.ToString(),
                Notes = notes.Select(n => new EmployeeNoteDto
                {
                    Id = n.Id,
                    NoteText = n.NoteText,
                    CreatedByUserName = n.CreatedByUserName,
                    CreatedTime = n.CreatedTime
                }).ToList(),
                LeaveHistory = leaveHistory.Select(lr => new LeaveRequestDto
                {
                    Id = lr.Id,
                    EmployeeId = lr.EmployeeId,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Type = lr.Type,
                    Status = lr.Status,
                    Description = lr.Description
                }).OrderByDescending(x => x.StartDate).ToList()
            };

            return Result<EmployeeDetailDto>.Success(dto);
        }

        private static string GetDepartmentName(Department dept)
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
