using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Domain.Enum;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Queries.GetAllLeaveRequest
{
    public class GetAllLeaveRequestsHandler : IRequestHandler<GetAllLeaveRequestsQuery, Result<IEnumerable<LeaveRequestDto>>>
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public GetAllLeaveRequestsHandler(
            ILeaveRequestRepository leaveRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<LeaveRequestDto>>> Handle(GetAllLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<LeaveRequest> leaveRequests;
            var role = _currentUser.Role ?? "";
            var userId = _currentUser.UserId;

            if (role == "Admin" || role == "IK")
            {
                leaveRequests = await _leaveRepository.GetAllAsync();
            }
            else if (role == "Yönetici" && _currentUser.DepartmentId.HasValue)
            {
                var deptEmployees = await _employeeRepository.GetByDepartmentAsync((Department)_currentUser.DepartmentId.Value);
                var employeeIds = deptEmployees.Select(e => e.Id).ToList();
                leaveRequests = await _leaveRepository.GetByEmployeeIdsAsync(employeeIds);
            }
            else if ((role == "Çalışan" || role == "Stajyer") && userId.HasValue)
            {
                leaveRequests = await _leaveRepository.GetByEmployeeIdAsync(userId.Value);
            }
            else
            {
                leaveRequests = new List<LeaveRequest>();
            }

            var leaveRequestDtos = _mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);
            return Result<IEnumerable<LeaveRequestDto>>.Success(leaveRequestDtos, "İzin talepleri yetki dahilinde başarıyla getirildi.");
        }
    }
}