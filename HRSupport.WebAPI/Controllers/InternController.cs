using HRSupport.Application.Features.Interns.Commands;
using HRSupport.Application.Features.Interns.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRSupport.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InternController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InternController> _logger;

        public InternController(IMediator mediator, ILogger<InternController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllInternsQuery());
            return Ok(result);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateInternCommand command)
        {
            var result = await _mediator.Send(command);
            _logger.LogInformation("Intern create endpoint, email: {Email}, success: {IsSuccess}", command.Email, result.IsSuccess);
            return Ok(result);
        }
    }
}
