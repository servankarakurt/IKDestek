using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Interns.Commands
{
    public class AddMentorNoteHandler : IRequestHandler<AddMentorNoteCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IMentorNoteRepository _mentorNoteRepository;
        private readonly ICurrentUserService _currentUser;

        public AddMentorNoteHandler(
            IInternRepository internRepository,
            IMentorNoteRepository mentorNoteRepository,
            ICurrentUserService currentUser)
        {
            _internRepository = internRepository;
            _mentorNoteRepository = mentorNoteRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<int>> Handle(AddMentorNoteCommand request, CancellationToken cancellationToken)
        {
            var role = _currentUser.Role ?? "";
            if (role != "Admin" && role != "IK" && role != "Yönetici" && role != "Çalışan")
                return Result<int>.Failure("Mentor notu ekleme yetkiniz yok.");

            var intern = await _internRepository.GetByIdAsync(request.InternId);
            if (intern == null)
                return Result<int>.Failure("Stajyer bulunamadı.");
            if (role == "Çalışan" && intern.MentorId != _currentUser.UserId)
                return Result<int>.Failure("Sadece kendi mentee'nize not ekleyebilirsiniz.");
            if (role == "Yönetici" && _currentUser.DepartmentId.HasValue &&
                (intern.Mentor == null || (int)intern.Mentor.Department != _currentUser.DepartmentId.Value))
                return Result<int>.Failure("Bu stajyere not ekleme yetkiniz yok.");

            if (string.IsNullOrWhiteSpace(request.NoteText))
                return Result<int>.Failure("Not metni zorunludur.");

            int? mentorId = null;
            if (role == "Çalışan")
                mentorId = intern.MentorId == _currentUser.UserId ? intern.MentorId : null;
            else if (role == "Admin" || role == "IK")
                mentorId = intern.MentorId;
            else if (role == "Yönetici")
                mentorId = intern.MentorId;

            var note = new MentorNote
            {
                InternId = request.InternId,
                MentorId = mentorId,
                NoteText = request.NoteText.Trim(),
                NoteDate = request.NoteDate
            };
            var id = await _mentorNoteRepository.AddAsync(note);
            return Result<int>.Success(id, "Mentor notu eklendi.");
        }
    }
}
