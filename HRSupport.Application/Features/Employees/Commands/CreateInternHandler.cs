using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Features.Interns.Commands;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class CreateInternHandler : IRequestHandler<CreateInternCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateInternHandler> _logger;

        public CreateInternHandler(
            IInternRepository internRepository,
            IMapper mapper,
            ILogger<CreateInternHandler> logger)
        {
            _internRepository = internRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateInternCommand request, CancellationToken cancellationToken)
        {
            var intern = _mapper.Map<Intern>(request);
            var internId = await _internRepository.AddAsync(intern);

            _logger.LogInformation("Intern created. InternId: {InternId}, Email: {Email}", internId, request.Email);
            return Result<int>.Success(internId, "Stajyer başarıyla eklendi.");
        }
    }
}
