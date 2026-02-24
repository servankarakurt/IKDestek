using HRSupport.Application.Features.Interns.Commands;
using HRSupport.Application.Features.Interns.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRSupport.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetInternByIdQuery(id));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateInternCommand command)
        {
            var result = await _mediator.Send(command);
            _logger.LogInformation("Intern create endpoint, email: {Email}, success: {IsSuccess}", command.Email, result.IsSuccess);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateInternCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteInternCommand(id));
            return Ok(result);
        }
    }
}
