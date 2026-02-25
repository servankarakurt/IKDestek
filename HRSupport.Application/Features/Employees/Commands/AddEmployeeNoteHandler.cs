using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class AddEmployeeNoteHandler : IRequestHandler<AddEmployeeNoteCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeNoteRepository _noteRepository;
        private readonly ICurrentUserService _currentUser;

        public AddEmployeeNoteHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeNoteRepository noteRepository,
            ICurrentUserService currentUser)
        {
            _employeeRepository = employeeRepository;
            _noteRepository = noteRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<int>> Handle(AddEmployeeNoteCommand request, CancellationToken cancellationToken)
        {
            var role = _currentUser.Role ?? "";
            if (role != "Admin" && role != "IK" && role != "Yönetici")
                return Result<int>.Failure("Sadece HR veya Yönetici not ekleyebilir.");

            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                return Result<int>.Failure("Çalışan bulunamadı.");
            if (role == "Yönetici" && _currentUser.DepartmentId.HasValue && (int)employee.Department != _currentUser.DepartmentId.Value)
                return Result<int>.Failure("Sadece kendi biriminizdeki çalışanlara not ekleyebilirsiniz.");

            if (string.IsNullOrWhiteSpace(request.NoteText))
                return Result<int>.Failure("Not metni boş olamaz.");

            var userId = _currentUser.UserId ?? 0;
            var userName = _currentUser.Email ?? "Sistem";

            var note = new EmployeeNote
            {
                EmployeeId = request.EmployeeId,
                NoteText = request.NoteText.Trim(),
                CreatedByUserId = userId,
                CreatedByUserName = userName
            };
            var id = await _noteRepository.AddAsync(note);
            return Result<int>.Success(id, "Not eklendi.");
        }
    }
}
