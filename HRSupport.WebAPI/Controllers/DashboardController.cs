using HRSupport.Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSupport.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}