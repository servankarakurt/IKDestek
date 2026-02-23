using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Auth.Commands
{
    
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<object>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService, ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                return Result<object>.Failure("E-posta veya şifre hatalı.");
            }

            var token = _tokenService.GenerateToken(user);
            _logger.LogInformation("Successful login for userId: {UserId}, email: {Email}", user.Id, user.Email);

            // 2. DİKKAT: Sadece düz token string'i dönmek yerine, UI'ın beklediği gibi 
            // "Token" ve "Email" özelliklerine sahip anonim bir nesne dönüyoruz.
            var responseData = new
            {
                Token = token,
                Email = user.Email
            };

            return Result<object>.Success(responseData, "Giriş başarılı.");
        }
    }
}