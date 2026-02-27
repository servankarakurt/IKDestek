using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Enum;
using MediatR;

namespace HRSupport.Application.Features.Employees.Queries.GetCollaeagues
{
    public class GetColleaguesHandler : IRequestHandler<GetColleaguesQuery, Result<IEnumerable<EmployeeDto>>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public GetColleaguesHandler(IEmployeeRepository employeeRepository, IMapper mapper, ICurrentUserService currentUser)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<EmployeeDto>>> Handle(GetColleaguesQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.DepartmentId.HasValue)
                return Result<IEnumerable<EmployeeDto>>.Success(Array.Empty<EmployeeDto>(), "Birim bilgisi yok.");

            var colleagues = await _employeeRepository.GetByDepartmentAsync((Department)_currentUser.DepartmentId.Value);
            var currentId = _currentUser.UserId;
            var list = colleagues.Where(e => !currentId.HasValue || e.Id != currentId.Value).ToList();
            var dtos = _mapper.Map<IEnumerable<EmployeeDto>>(list);
            return Result<IEnumerable<EmployeeDto>>.Success(dtos, "Birim arkadaşları getirildi.");
        }
    }
}
