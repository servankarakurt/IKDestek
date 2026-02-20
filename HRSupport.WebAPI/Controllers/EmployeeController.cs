using HRSupport.Application.Features.Employees.Commans;
using HRSupport.Application.Features.Employees.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRSupport.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sınıf seviyesinde sisteme login olmak zorunlu
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllEmployeesQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
            return Ok(result);
        }

        // DİKKAT: Sadece Admin(1) ve İK(2) yeni personel ekleyebilir.
        [HttpPost("create")]
        [Authorize(Roles = "1, 2")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DİKKAT: Güncelleme işlemi de genelde İK ve Admin'dedir.
        [HttpPut("update")]
        [Authorize(Roles = "1, 2")]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DİKKAT: Silme yetkisi sadece İK ve Admin'de.
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "1, 2")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteEmployeeCommand(id));
            return Ok(result);
        }
    }
}