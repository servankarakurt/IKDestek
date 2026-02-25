using HRSupport.Application.Features.Employees.Commands;
using HRSupport.Application.Features.Employees.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRSupport.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IMediator mediator, ILogger<EmployeeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>Aynı birimdeki çalışanlar (Çalışan rolü paneli için).</summary>
        [HttpGet("colleagues")]
        public async Task<IActionResult> GetColleagues()
        {
            var result = await _mediator.Send(new GetColleaguesQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,IK,Yönetici")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _mediator.Send(new GetAllEmployeesQuery());
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee list");
                return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("{id}/detail")]
        [Authorize(Roles = "Admin,IK,Yönetici")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _mediator.Send(new GetEmployeeDetailQuery(id));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,IK,Yönetici")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPost("notes")]
        [Authorize(Roles = "Admin,IK,Yönetici")]
        public async Task<IActionResult> AddNote([FromBody] AddEmployeeNoteCommand command)
        {
            if (command == null) return BadRequest();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,IK")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            try
            {
                if (command == null)
                {
                    return BadRequest(new { error = "Geçersiz veri", isSuccess = false });
                }

                var result = await _mediator.Send(command);
                _logger.LogInformation("Employee create endpoint, email: {Email}, success: {IsSuccess}", command.Email, result.IsSuccess);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Employee create hatası");
                return BadRequest(new { error = $"Sunucu hatası: {ex.Message}", isSuccess = false });
            }
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin,IK")]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin,IK")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteEmployeeCommand(id));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
