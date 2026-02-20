using HRSupport.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HRSupport.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            // Başarısız ise 401 Unauthorized dönüyoruz
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("change-password")]
        [Authorize] // Şifre değiştirmek için sisteme geçici token ile girmiş olması gerek
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            // Token'dan giriş yapan kişinin ID'sini alıyoruz (Güvenlik için)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                command.UserId = userId; // ID'yi DTO'ya basıyoruz
                var result = await _mediator.Send(command);
                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return Unauthorized();
        }
    }
}