using AutoMapper;
using BCrypt.Net; 
using HRSupport.Application.Common;
using HRSupport.Application.Features.Employees.Commans;
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
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Result<int>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository; // EKLENDİ: User tablosuna erişim için
        private readonly IMapper _mapper;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // 1. Personel (Employee) Kaydını Oluştur
            var employee = _mapper.Map<Employee>(request);
            var employeeId = await _employeeRepository.AddAsync(employee);

            // 2. Rastgele 8 Haneli Geçici Şifre Üret
            string tempPassword = GenerateTemporaryPassword();

            // 3. Şifreyi Güvenli Hale Getir (Hash'le)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

            // 4. Sisteme Giriş Yapabilmesi İçin Yeni User Hesabı Oluştur
            var newUser = new User
            {
                Email = request.Email, // EmployeeCommand'den gelen email
                PasswordHash = passwordHash,
                Role =Roles.Çalışan, // Default olarak çalışan rolü veriyoruz
                IsPasswordChangeRequired = true   // 3. aşamada bu true ise şifre değiştirticez
            };

            // DİKKAT: IUserRepository içine AddAsync metodunu eklemediysen hata verebilir, eklemelisin.
            await _userRepository.AddAsync(newUser);

            // 5. İK Uzmanına geçici şifreyi UI'da gösterebilmek için mesaj olarak dönüyoruz
            string message = $"Personel başarıyla eklendi. Geçici Şifre: {tempPassword} (Lütfen bu şifreyi personele iletin.)";

            return Result<int>.Success(employeeId, message);
        }

        // Rastgele şifre üreten yardımcı metot
        private string GenerateTemporaryPassword()
        {
            var chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}