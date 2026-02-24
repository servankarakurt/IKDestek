using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Interns.Commands
{
    public class DeleteInternHandler : IRequestHandler<DeleteInternCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;

        public DeleteInternHandler(IInternRepository internRepository)
        {
            _internRepository = internRepository;
        }

        public async Task<Result<int>> Handle(DeleteInternCommand request, CancellationToken cancellationToken)
        {
            var existingIntern = await _internRepository.GetByIdAsync(request.Id);

            if (existingIntern == null)
            {
                return Result<int>.Failure("Silinmek istenen stajyer bulunamadı veya zaten silinmiş.");
            }

            await _internRepository.DeleteAsync(request.Id);
            return Result<int>.Success(request.Id, "Stajyer başarıyla silindi.");
        }
    }
}

