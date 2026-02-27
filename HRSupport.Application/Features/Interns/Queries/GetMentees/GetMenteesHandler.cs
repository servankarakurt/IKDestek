using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;

namespace HRSupport.Application.Features.Interns.Queries.GetMentees
{
    public class GetMenteesHandler : IRequestHandler<GetMenteesQuery, Result<IEnumerable<InternDto>>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public GetMenteesHandler(IInternRepository internRepository, IMapper mapper, ICurrentUserService currentUser)
        {
            _internRepository = internRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<InternDto>>> Handle(GetMenteesQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.UserId.HasValue)
                return Result<IEnumerable<InternDto>>.Success(Array.Empty<InternDto>(), "Kullanıcı bilgisi yok.");

            var mentees = await _internRepository.GetByMentorIdAsync(_currentUser.UserId.Value);
            var dtos = _mapper.Map<IEnumerable<InternDto>>(mentees);
            return Result<IEnumerable<InternDto>>.Success(dtos, "Mentee listesi getirildi.");
        }
    }
}
