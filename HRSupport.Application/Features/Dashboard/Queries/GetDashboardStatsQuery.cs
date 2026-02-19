using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using MediatR;

namespace HRSupport.Application.Features.Dashboard.Queries
{
    public class GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>
    {
        // Bu sorgu dışarıdan bir parametre almayacak, sadece istatistik getirecek.
    }
}