using HRSupport.Application.Common;
using HRSupport.Application.Interfaces; // Repository interface'i için
using HRSupport.Domain.Entites;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Reports.Queries
{
    public class GetMyReportRequestsHandler : IRequestHandler<GetMyReportRequestsQuery, Result<IEnumerable<WeeklyReportRequest>>>
    {
        // Not: IWeeklyReportRepository'yi oluşturmuş olman gerekir.
        // Eğer henüz oluşturmadıysan IEmployeeRepository gibi bir yapı kurmalısın.
        private readonly IWeeklyReportRepository _reportRepository;

        public GetMyReportRequestsHandler(IWeeklyReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Result<IEnumerable<WeeklyReportRequest>>> Handle(GetMyReportRequestsQuery request, CancellationToken cancellationToken)
        {
            // 1. Repository üzerinden çalışana ait rapor taleplerini çekiyoruz.
            // Bu metodun repository katmanında tanımlı olması gerekir.
            var reports = await _reportRepository.GetByEmployeeIdAsync(request.EmployeeId);

            // 2. Eğer liste boşsa bile boş bir koleksiyon dönmek (null yerine) UI tarafında hata almanı engeller.
            if (reports == null)
            {
                return Result<IEnumerable<WeeklyReportRequest>>.Success(new List<WeeklyReportRequest>(), "Henüz bir rapor talebiniz bulunmamaktadır.");
            }

            return Result<IEnumerable<WeeklyReportRequest>>.Success(reports, "Rapor talepleri başarıyla getirildi.");
        }
    }
}