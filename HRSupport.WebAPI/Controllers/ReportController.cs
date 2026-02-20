using HRSupport.Application.Features.Reports.Commands;
using HRSupport.Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace HRSupport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        [Authorize(Roles = "Yönetici")]
        public async Task<IActionResult> RequestReport([FromBody] CreateReportRequestCommand command)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            command.ManagerUserId = userId;
            var result = await _mediator.Send(command);

            _logger.LogInformation("UserId {UserId} report request endpoint sonucu: {IsSuccess}", userId, result.IsSuccess);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("my-requests")]
        [Authorize(Roles = "Çalışan")]
        public async Task<IActionResult> GetMyReportRequests()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var query = new GetMyReportRequestsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
