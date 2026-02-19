using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetAllLeaveRequestsHandler : IRequestHandler<GetAllLeaveRequestsQuery, Result<IEnumerable<LeaveRequestDto>>>
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IMapper _mapper;

        public GetAllLeaveRequestsHandler(ILeaveRequestRepository leaveRepository, IMapper mapper)
        {
            _leaveRepository = leaveRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<LeaveRequestDto>>> Handle(GetAllLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            // Veritabanındaki tüm izin taleplerini çekiyoruz
            var leaveRequests = await _leaveRepository.GetAllAsync();

            // Gelen Entity listesini AutoMapper ile DTO listesine dönüştürüyoruz
            var leaveRequestDtos = _mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);

            return Result<IEnumerable<LeaveRequestDto>>.Success(leaveRequestDtos, "Tüm izin talepleri başarıyla getirildi.");
        }
    }
}