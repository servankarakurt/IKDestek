using HRSupport.Application.Features.LeaveRequests.Commands;
using HRSupport.Application.Features.LeaveRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRSupport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateLeaveRequestStatusCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllLeaveRequestsQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetLeaveRequestByIdQuery(id));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>İzin formu yazdırma için gerekli bilgiler. Personel kendi iznini yazdırabilir.</summary>
        [HttpGet("{id}/print-info")]
        public async Task<IActionResult> GetPrintInfo(int id)
        {
            var result = await _mediator.Send(new GetLeaveRequestPrintInfoQuery(id));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateLeaveRequestCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("my-balance")]
        public async Task<IActionResult> GetMyLeaveBalance()
        {
            var result = await _mediator.Send(new GetMyLeaveBalanceQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}