using HRSupport.Application.Features.Reports.Commands;
using HRSupport.Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRSupport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Yöneticilerin (Role 4) rapor talep etmesi için
        [HttpPost("request-report")]
        [Authorize(Roles = "4,Yönetici")]
        public async Task<IActionResult> RequestReport([FromBody] CreateReportRequestCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // Personel ve Stajyerlerin (Role 3 ve 5) kendilerinden istenen raporları görmesi için
        [HttpGet("my-requests")]
        [Authorize(Roles = "3,5,Çalışan,Stajyer")]
        public async Task<IActionResult> GetMyReportRequests()
        {
            // Token içerisinden login olan kullanıcının ID'sini alıyoruz
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = new GetMyReportRequestsQuery { EmployeeId = int.Parse(userId) };
            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}