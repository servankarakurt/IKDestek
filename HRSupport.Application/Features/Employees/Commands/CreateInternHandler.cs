using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Features.Interns.Commands;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

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

            // Generate temporary password and store hash
            var tempPassword = GenerateTemporaryPassword(10);
            intern.PasswordHash = HashPassword(tempPassword);
            intern.MustChangePassword = true;

            var internId = await _internRepository.AddAsync(intern);

            _logger.LogInformation("Intern created. InternId: {InternId}, Email: {Email}", internId, request.Email);

            var message = $"Geçici şifre: {tempPassword}. Kullanıcı ilk girişte değiştirmelidir.";
            return Result<int>.Success(internId, message);
        }

        private static string GenerateTemporaryPassword(int length = 10)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_";
            var sb = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[4];
            while (sb.Length < length)
            {
                rng.GetBytes(buffer);
                var num = BitConverter.ToUInt32(buffer, 0);
                var idx = (int)(num % (uint)chars.Length);
                sb.Append(chars[idx]);
            }
            return sb.ToString();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
