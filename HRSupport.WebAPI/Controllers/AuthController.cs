using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSupport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand? command)
        {
            if (command == null)
                return BadRequest(Result<LoginResponseDto>.Failure("E-posta ve şifre gerekli."));
            _logger.LogInformation("Login isteği alındı: {Email}", command.Email);
            var result = await _mediator.Send(command);
            // 401 yerine 400: yanlış şifre "Authorization gerekli" gibi yorumlanmasın
            if (result.IsSuccess)
                _logger.LogInformation("Login başarılı: {Email}", result.Value?.Email);
            else
                _logger.LogWarning("Login başarısız: {Error}", result.Error);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
