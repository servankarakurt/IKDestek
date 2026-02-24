using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Interns.Queries
{
    public class GetInternByIdHandler : IRequestHandler<GetInternByIdQuery, Result<InternDto>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IMapper _mapper;

        public GetInternByIdHandler(IInternRepository internRepository, IMapper mapper)
        {
            _internRepository = internRepository;
            _mapper = mapper;
        }

        public async Task<Result<InternDto>> Handle(GetInternByIdQuery request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository.GetByIdAsync(request.Id);

            if (intern == null)
            {
                return Result<InternDto>.Failure("Belirtilen Id'ye sahip stajyer bulunamadı.");
            }

            var dto = _mapper.Map<InternDto>(intern);
            return Result<InternDto>.Success(dto, "Stajyer başarıyla getirildi.");
        }
    }
}

