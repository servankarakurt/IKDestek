using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeByIdHandler(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.Id);

            if (employee == null)
            {
                return Result<EmployeeDto>.Failure("Belirtilen Id'ye sahip çalışan bulunamadı.");
            }

            // Tekil Entity'yi DTO'ya dönüştürüyoruz
            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return Result<EmployeeDto>.Success(employeeDto, "Çalışan başarıyla getirildi.");
        }
    }
}