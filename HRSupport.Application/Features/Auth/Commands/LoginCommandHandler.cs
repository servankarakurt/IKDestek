using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HRSupport.Application.Features.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
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

        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                return Result<string>.Failure("E-posta veya şifre hatalı.");
            }

            var token = _tokenService.GenerateToken(user);
            _logger.LogInformation("Successful login for userId: {UserId}, email: {Email}", user.Id, user.Email);

            return Result<string>.Success(token, "Giriş başarılı.");
        }
    }
}
