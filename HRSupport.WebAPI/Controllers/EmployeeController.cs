namespace HRSupport.WebAPI.Controllers
{
    using HRSupport.Application.Features.Employees.Commans;
    using HRSupport.Application.Features.Employees.Queries;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var query = new GetAllEmployeesQuery();
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(result); 
            }
            else
            {
                return BadRequest(result.Error);
            }
        }
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
     
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var command = new DeleteEmployeeCommand(id);
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