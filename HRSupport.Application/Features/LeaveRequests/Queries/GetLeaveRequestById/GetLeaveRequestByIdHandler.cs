using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Queries.GetLeaveRequestById
{
    public class GetLeaveRequestByIdHandler : IRequestHandler<GetLeaveRequestByIdQuery, Result<LeaveRequestDto>>
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public GetLeaveRequestByIdHandler(
            ILeaveRequestRepository leaveRepository,
            IEmployeeRepository employeeRepository,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<Result<LeaveRequestDto>> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null)
                return Result<LeaveRequestDto>.Failure("İzin talebi bulunamadı.");

            var role = _currentUser.Role ?? string.Empty;
            var userId = _currentUser.UserId;
            var departmentId = _currentUser.DepartmentId;

            if (role == "Admin" || role == "IK")
            {
                // Tam yetki
            }
            else if (role == "Yönetici")
            {
                var employee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
                if (employee == null || !departmentId.HasValue || (int)employee.Department != departmentId.Value)
                    return Result<LeaveRequestDto>.Failure("Bu izin talebini görüntüleme yetkiniz yok.");
            }
            else if (role == "Çalışan" || role == "Stajyer")
            {
                if (!userId.HasValue || leaveRequest.EmployeeId != userId.Value)
                    return Result<LeaveRequestDto>.Failure("Sadece kendi izin talebinizi görüntüleyebilirsiniz.");
            }
            else
            {
                return Result<LeaveRequestDto>.Failure("Bu işlem için yetkiniz yok.");
            }

            var dto = _mapper.Map<LeaveRequestDto>(leaveRequest);
            return Result<LeaveRequestDto>.Success(dto);
        }
    }
}
