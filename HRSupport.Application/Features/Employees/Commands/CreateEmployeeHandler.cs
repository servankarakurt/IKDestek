using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Features.Employees.Commans;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateEmployeeHandler> _logger;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            ILogger<CreateEmployeeHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Employee>(request);
            var employeeId = await _employeeRepository.AddAsync(employee);

            _logger.LogInformation("Employee created. EmployeeId: {EmployeeId}, Email: {Email}", employeeId, request.Email);

            return Result<int>.Success(employeeId, "Personel başarıyla eklendi.");
        }
    }
}
