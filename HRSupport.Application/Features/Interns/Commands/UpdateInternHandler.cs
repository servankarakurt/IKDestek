using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Interns.Commands
{
    public class UpdateInternHandler : IRequestHandler<UpdateInternCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;

        public UpdateInternHandler(IInternRepository internRepository)
        {
            _internRepository = internRepository;
        }

        public async Task<Result<int>> Handle(UpdateInternCommand request, CancellationToken cancellationToken)
        {
            var existingIntern = await _internRepository.GetByIdAsync(request.Id);

            if (existingIntern == null)
            {
                return Result<int>.Failure("Güncellenmek istenen stajyer bulunamadı.");
            }

            existingIntern.FirstName = request.FirstName;
            existingIntern.LastName = request.LastName;
            existingIntern.Email = request.Email;
            existingIntern.Phone = request.Phone;
            existingIntern.University = request.University;
            existingIntern.Major = request.Major;
            existingIntern.Grade = request.Grade;
            existingIntern.StartDate = request.StartDate;
            existingIntern.EndDate = request.EndDate;
            existingIntern.MentorId = request.MentorId;

            await _internRepository.UpdateAsync(existingIntern);

            return Result<int>.Success(existingIntern.Id, "Stajyer bilgileri başarıyla güncellendi.");
        }
    }
}

