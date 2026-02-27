using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Interns.Queries.GetInternDetail
{
    public class GetInternDetailHandler : IRequestHandler<GetInternDetailQuery, Result<InternDetailDto>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IInternTaskRepository _taskRepository;
        private readonly IMentorNoteRepository _mentorNoteRepository;
        private readonly ICurrentUserService _currentUser;

        public GetInternDetailHandler(
            IInternRepository internRepository,
            IInternTaskRepository taskRepository,
            IMentorNoteRepository mentorNoteRepository,
            ICurrentUserService currentUser)
        {
            _internRepository = internRepository;
            _taskRepository = taskRepository;
            _mentorNoteRepository = mentorNoteRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<InternDetailDto>> Handle(GetInternDetailQuery request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository.GetByIdAsync(request.Id);
            if (intern == null)
                return Result<InternDetailDto>.Failure("Stajyer bulunamadı.");

            var role = _currentUser.Role ?? "";
            if (role != "Admin" && role != "IK" && role != "Yönetici" && role != "Çalışan")
            {
                if (role == "Stajyer" && _currentUser.UserId != intern.Id)
                    return Result<InternDetailDto>.Failure("Sadece kendi detayınızı görüntüleyebilirsiniz.");
                if (role != "Stajyer")
                    return Result<InternDetailDto>.Failure("Bu işlem için yetkiniz yok.");
            }

            var tasks = await _taskRepository.GetByInternIdAsync(request.Id);
            var mentorNotes = await _mentorNoteRepository.GetByInternIdAsync(request.Id);

            var dto = new InternDetailDto
            {
                Id = intern.Id,
                FirstName = intern.FirstName,
                LastName = intern.LastName,
                FullName = intern.FullName,
                Email = intern.Email,
                Phone = intern.Phone,
                University = intern.University,
                Major = intern.Major,
                Grade = intern.Grade,
                StartDate = intern.StartDate,
                EndDate = intern.EndDate,
                MentorId = intern.MentorId,
                MentorName = intern.Mentor != null ? intern.Mentor.FullName : null,
                Tasks = tasks.Select(t => new InternTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    CreatedTime = t.CreatedTime
                }).ToList(),
                MentorNotes = mentorNotes.Select(m => new MentorNoteDto
                {
                    Id = m.Id,
                    NoteText = m.NoteText,
                    NoteDate = m.NoteDate,
                    CreatedTime = m.CreatedTime,
                    MentorName = m.Mentor?.FullName
                }).ToList()
            };

            return Result<InternDetailDto>.Success(dto);
        }
    }
}
