using HRSupport.Application.Features.Reports.Commands;
using HRSupport.Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRSupport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IMediator mediator, ILogger<ReportController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("request-report")]
        public async Task<IActionResult> RequestReport([FromBody] CreateReportRequestCommand command)
        {
            var result = await _mediator.Send(command);

            _logger.LogInformation("Report request endpoint sonucu: {IsSuccess}", result.IsSuccess);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("my-requests/{employeeId}")]
        public async Task<IActionResult> GetMyReportRequests(int employeeId)
        {
            var query = new GetMyReportRequestsQuery { EmployeeId = employeeId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
