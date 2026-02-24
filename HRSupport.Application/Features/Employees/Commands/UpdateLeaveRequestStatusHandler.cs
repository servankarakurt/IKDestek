using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Domain.Enum;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Commans
{
    public class UpdateLeaveRequestStatusHandler : IRequestHandler<UpdateLeaveRequestStatusCommand, Result<bool>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public UpdateLeaveRequestStatusHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<Result<bool>> Handle(UpdateLeaveRequestStatusCommand request, CancellationToken cancellationToken)
        {
            // 1. İzin talebini veritabanından buluyoruz
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);

            if (leaveRequest == null)
            {
                return Result<bool>.Failure("İzin talebi bulunamadı.");
            }

            // 2. İzin durumunu UI'dan (Butondan) gelen yeni durumla güncelliyoruz
            leaveRequest.Status = request.NewStatus;

            // 3. ONAY GEÇMİŞİ LOGU: Bu işlemin kim tarafından ne zaman yapıldığını kaydediyoruz
            var historyRecord = new LeaveApprovalHistory
            {
                LeaveRequestId = leaveRequest.Id,
                ActionDate = DateTime.Now,
                Action = request.NewStatus,
                // Red veya Revizyon ise açıklamayı ekle, değilse standart mesaj yaz.
                Comments = request.RejectReason ?? (request.NewStatus == LeaveStatus.Onaylandı ? "İzin Onaylandı." : "Durum Güncellendi."),
                // Not: Şimdilik Yetkilendirme (Auth) yapmadığımız için ID'yi sabit (1) veriyoruz.
                // Auth adımına geçtiğimizde burayı User.Identity.GetUserId() gibi bir yapıyla dinamik yapacağız.
                ProcessedByUserId = 1
            };
            leaveRequest.ApprovalHistories.Add(historyRecord);
            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            return Result<bool>.Success(true);
        }
    }
}