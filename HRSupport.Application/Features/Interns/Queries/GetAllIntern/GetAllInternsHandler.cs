using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Interns.Queries.GetAllIntern
{
    public class GetAllInternsHandler : IRequestHandler<GetAllInternsQuery, Result<IEnumerable<InternDto>>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IMapper _mapper;

        public GetAllInternsHandler(IInternRepository internRepository, IMapper mapper)
        {
            _internRepository = internRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<InternDto>>> Handle(GetAllInternsQuery request, CancellationToken cancellationToken)
        {
            var interns = await _internRepository.GetAllAsync();
            var internDtos = _mapper.Map<IEnumerable<InternDto>>(interns);

            return Result<IEnumerable<InternDto>>.Success(internDtos, "Stajyer listesi başarıyla getirildi.");
        }
    }
}