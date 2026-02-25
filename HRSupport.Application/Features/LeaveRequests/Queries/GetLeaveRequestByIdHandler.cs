using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestByIdHandler : IRequestHandler<GetLeaveRequestByIdQuery, Result<LeaveRequestDto>>
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IMapper _mapper;

        public GetLeaveRequestByIdHandler(ILeaveRequestRepository leaveRepository, IMapper mapper)
        {
            _leaveRepository = leaveRepository;
            _mapper = mapper;
        }

        public async Task<Result<LeaveRequestDto>> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null)
                return Result<LeaveRequestDto>.Failure("İzin talebi bulunamadı.");
            var dto = _mapper.Map<LeaveRequestDto>(leaveRequest);
            return Result<LeaveRequestDto>.Success(dto);
        }
    }
}
