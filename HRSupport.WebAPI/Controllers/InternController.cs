using HRSupport.Application.Features.Employees.Commands;
using HRSupport.Application.Features.Employees.Queries;
using HRSupport.Application.Features.Interns.Commands;
using HRSupport.Application.Features.Interns.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRSupport.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InternController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllInternsQuery());
            return Ok(result);
        }

        // DİKKAT: Sadece Admin(1) ve İK(2) stajyer ekleyebilir.
        [HttpPost("create")]
        [Authorize(Roles = "1, 2")]
        public async Task<IActionResult> Create([FromBody] CreateInternCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}