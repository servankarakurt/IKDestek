using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Interns.Commands.AddInternTask
{
    public class AddInternTaskHandler : IRequestHandler<AddInternTaskCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IInternTaskRepository _taskRepository;
        private readonly ICurrentUserService _currentUser;

        public AddInternTaskHandler(
            IInternRepository internRepository,
            IInternTaskRepository taskRepository,
            ICurrentUserService currentUser)
        {
            _internRepository = internRepository;
            _taskRepository = taskRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<int>> Handle(AddInternTaskCommand request, CancellationToken cancellationToken)
        {
            var role = _currentUser.Role ?? "";
            if (role != "Admin" && role != "IK" && role != "Yönetici" && role != "Çalışan")
                return Result<int>.Failure("Görev ekleme yetkiniz yok.");

            var intern = await _internRepository.GetByIdAsync(request.InternId);
            if (intern == null)
                return Result<int>.Failure("Stajyer bulunamadı.");
            if (role == "Çalışan" && intern.MentorId != _currentUser.UserId)
                return Result<int>.Failure("Sadece kendi mentee'nize görev ekleyebilirsiniz.");
            if (role == "Yönetici" && _currentUser.DepartmentId.HasValue &&
                (intern.Mentor == null || (int)intern.Mentor.Department != _currentUser.DepartmentId.Value))
                return Result<int>.Failure("Bu stajyere görev ekleme yetkiniz yok.");

            if (string.IsNullOrWhiteSpace(request.Title))
                return Result<int>.Failure("Görev başlığı zorunludur.");

            var task = new InternTask
            {
                InternId = request.InternId,
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                IsCompleted = false
            };
            var id = await _taskRepository.AddAsync(task);
            return Result<int>.Success(id, "Görev eklendi.");
        }
    }
}
