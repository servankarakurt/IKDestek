using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;

        public AuthController(IMediator mediator, ILogger<AuthController> logger, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Sadece Development ortamında: verilen şifre için BCrypt hash üretir.
        /// Kullanım: GET /api/Auth/dev-hash?password=Admin123!
        /// Dönen hash'i Users veya Employees tablosunda PasswordHash kolonuna yazmak için kullanın.
        /// </summary>
        [HttpGet("dev-hash")]
        public IActionResult GetDevHash([FromQuery] string? password)
        {
            if (!_env.IsDevelopment())
                return NotFound();
            if (string.IsNullOrEmpty(password))
                return BadRequest("password parametresi gerekli (örn. ?password=Admin123!)");
            var hash = PasswordHelper.Hash(password);
            return Ok(new { hash, passwordLength = password.Length });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand? command)
        {
            if (command == null)
                return BadRequest(Result<LoginResponseDto>.Failure("E-posta ve şifre gerekli."));
            _logger.LogInformation("Login isteği alındı: {Email}", command.Email);
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                _logger.LogInformation("Login başarılı: {Email}", result.Value?.Email);
            else
                _logger.LogWarning("Login başarısız: {Error}", result.Error);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
