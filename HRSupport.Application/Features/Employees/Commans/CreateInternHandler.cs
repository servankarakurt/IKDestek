using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.Features.Interns.Commands;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Common;
using HRSupport.Domain.Entites;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Commands
{
    public class CreateInternHandler : IRequestHandler<CreateInternCommand, Result<int>>
    {
        private readonly IInternRepository _internRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateInternHandler(
            IInternRepository internRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _internRepository = internRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateInternCommand request, CancellationToken cancellationToken)
        {
            var intern = _mapper.Map<Intern>(request);
            var internId = await _internRepository.AddAsync(intern);

            // Geçici şifre üret ve hashle
            string tempPassword = GenerateTemporaryPassword();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

            // Stajyer için User hesabı oluştur
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = Roles.Stajyer, // Stajyer rolü atandı
                IsPasswordChangeRequired = true
            };

            await _userRepository.AddAsync(newUser);

            return Result<int>.Success(internId, $"Stajyer eklendi. Geçici Şifre: {tempPassword}");
        }

        private string GenerateTemporaryPassword()
        {
            var chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}