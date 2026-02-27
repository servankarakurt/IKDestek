using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;

namespace HRSupport.Application.Features.Interns.Queries.GetMyMentor
{
    public class GetMyMentorHandler : IRequestHandler<GetMyMentorQuery, Result<MentorInfoDto?>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUser;

        public GetMyMentorHandler(IInternRepository internRepository, IEmployeeRepository employeeRepository, ICurrentUserService currentUser)
        {
            _internRepository = internRepository;
            _employeeRepository = employeeRepository;
            _currentUser = currentUser;
        }

        public async Task<Result<MentorInfoDto?>> Handle(GetMyMentorQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.Role != "Stajyer")
                return Result<MentorInfoDto?>.Success(null, "Mentor bilgisi yok.");

            var intern = _currentUser.UserId.HasValue
                ? await _internRepository.GetByIdAsync(_currentUser.UserId.Value)
                : null;
            if (intern == null && !string.IsNullOrWhiteSpace(_currentUser.Email))
                intern = await _internRepository.GetByEmailAsync(_currentUser.Email);
            if (intern?.MentorId == null)
                return Result<MentorInfoDto?>.Success(null, "Mentor atanmamış.");

            var mentor = await _employeeRepository.GetByIdAsync(intern.MentorId.Value);
            if (mentor == null)
                return Result<MentorInfoDto?>.Success(null, "Mentor bulunamadı.");

            var dto = new MentorInfoDto
            {
                Id = mentor.Id,
                FullName = mentor.FullName,
                Email = mentor.Email,
                Phone = mentor.Phone ?? ""
            };
            return Result<MentorInfoDto?>.Success(dto, "Mentor getirildi.");
        }
    }
}
