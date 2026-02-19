using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Interns.Commands
{
    public class CreateInternHandler : IRequestHandler<CreateInternCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;

        public CreateInternHandler(IInternRepository internRepository)
        {
            _internRepository = internRepository;
        }

        public async Task<Result<int>> Handle(CreateInternCommand request, CancellationToken cancellationToken)
        {
            var intern = new Intern
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                University = request.University,
                Major = request.Major,
                Grade = request.Grade,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MentorId = request.MentorId
            };

            var id = await _internRepository.AddAsync(intern);
            return Result<int>.Success(id, "Stajyer kaydı başarıyla oluşturuldu.");
        }
    }
}