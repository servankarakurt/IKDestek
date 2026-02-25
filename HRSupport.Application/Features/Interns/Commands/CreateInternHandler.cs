using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace HRSupport.Application.Features.Interns.Commands
{
    public class CreateInternHandler : IRequestHandler<CreateInternCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IEmployeeLeaveBalanceRepository _balanceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateInternHandler> _logger;

        public CreateInternHandler(
            IInternRepository internRepository,
            IEmployeeLeaveBalanceRepository balanceRepository,
            IMapper mapper,
            ILogger<CreateInternHandler> logger)
        {
            _internRepository = internRepository;
            _balanceRepository = balanceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateInternCommand request, CancellationToken cancellationToken)
        {
            var intern = _mapper.Map<Intern>(request);

            // E-posta tutarlı arama için küçük harf ve trim (girişte aynı normalleştirme kullanılıyor)
            if (!string.IsNullOrWhiteSpace(intern.Email))
                intern.Email = intern.Email.Trim().ToLowerInvariant();

            // Geçici şifre: sadece alfanumerik (kopyala-yapıştır ve giriş sorunlarını önlemek için)
            var tempPassword = GenerateTemporaryPassword(10);
            intern.PasswordHash = PasswordHelper.Hash(tempPassword);
            intern.MustChangePassword = true;

            var internId = await _internRepository.AddAsync(intern);

            await _balanceRepository.AddAsync(new EmployeeLeaveBalance
            {
                EmployeeId = internId,
                RemainingAnnualLeaveDays = 20
            });

            _logger.LogInformation("Intern created. InternId: {InternId}, Email: {Email}", internId, request.Email);

            var message = $"Geçici şifre: {tempPassword}. Kullanıcı ilk girişte değiştirmelidir.";
            return Result<int>.Success(internId, message);
        }

        private static string GenerateTemporaryPassword(int length = 10)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
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

    }
}
