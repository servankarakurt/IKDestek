namespace HRSupport.WebAPI.Controllers
{
    using HRSupport.Application.Features.Employees.Commans;
    using HRSupport.Application.Features.Employees.Queries;
    using HRSupport.Domain.Common;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await _mediator.Send(new GetAllEmployeesQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Create")]
      [Authorize(Roles = "Admin,HR")] 
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,IK")] 
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _mediator.Send(new DeleteEmployeeCommand(id));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    
        [HttpGet]
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var query = new GetEmployeeByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {             
                return NotFound(result.Error);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Error);
            }
        } 
    }
}