using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Features.Interns.Commands;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Common;
using HRSupport.Domain.Entites;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class CreateInternHandler : IRequestHandler<CreateInternCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateInternHandler> _logger;

        public CreateInternHandler(
            IInternRepository internRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<CreateInternHandler> logger)
        {
            _internRepository = internRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateInternCommand request, CancellationToken cancellationToken)
        {
            var intern = _mapper.Map<Intern>(request);
            var internId = await _internRepository.AddAsync(intern);

            var tempPassword = GenerateTemporaryPassword();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = Roles.Stajyer,
                IsPasswordChangeRequired = true
            };

            await _userRepository.AddAsync(newUser);

            _logger.LogInformation("Intern created. InternId: {InternId}, Email: {Email}", internId, request.Email);
            return Result<int>.Success(internId, $"Stajyer eklendi. Geçici Şifre: {tempPassword} (İlk girişte şifre değişikliği zorunludur.)");
        }

        private string GenerateTemporaryPassword()
        {
            var chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
