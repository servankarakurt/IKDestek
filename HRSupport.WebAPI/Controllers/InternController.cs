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

        /// <summary>Stajyerin kendi mentor bilgisi (ad, e-posta, telefon).</summary>
        [HttpGet("my-mentor")]
        public async Task<IActionResult> GetMyMentor()
        {
            var result = await _mediator.Send(new GetMyMentorQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>Mentor olan çalışanın kendi mentee listesi (Çalışan/Yönetici/Admin/IK).</summary>
        [HttpGet("mentees")]
        public async Task<IActionResult> GetMentees()
        {
            var result = await _mediator.Send(new GetMenteesQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,IK")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllInternsQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/detail")]
        [Authorize]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _mediator.Send(new GetInternDetailQuery(id));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,IK")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetInternByIdQuery(id));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPost("tasks")]
        [Authorize(Roles = "Admin,IK,Yönetici,Çalışan")]
        public async Task<IActionResult> AddTask([FromBody] AddInternTaskCommand command)
        {
            if (command == null) return BadRequest();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("mentor-notes")]
        [Authorize(Roles = "Admin,IK,Yönetici,Çalışan")]
        public async Task<IActionResult> AddMentorNote([FromBody] AddMentorNoteCommand command)
        {
            if (command == null) return BadRequest();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,IK")]
        public async Task<IActionResult> Create([FromBody] CreateInternCommand command)
        {
            var result = await _mediator.Send(command);
            _logger.LogInformation("Intern create endpoint, email: {Email}, success: {IsSuccess}", command.Email, result.IsSuccess);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin,IK")]
        public async Task<IActionResult> Update([FromBody] UpdateInternCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin,IK")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteInternCommand(id));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
