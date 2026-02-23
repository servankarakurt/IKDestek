using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Commans
{
    public class UpdateLeaveRequestStatusHandler : IRequestHandler<UpdateLeaveRequestStatusCommand, Result<bool>>
    {
        private readonly ILeaveRequestRepository _leaveRepository;

        public UpdateLeaveRequestStatusHandler(ILeaveRequestRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
        }

        public async Task<Result<bool>> Handle(UpdateLeaveRequestStatusCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRepository.GetByIdAsync(request.Id);

            if (leaveRequest == null)
            {
                return Result<bool>.Failure("İzin talebi bulunamadı.");
            }

            // Durumu güncelle
            leaveRequest.Status = request.NewStatus;

            // Eğer açıklama geldiyse (red durumu gibi), mevcut açıklamaya ekle veya güncelle
            if (!string.IsNullOrEmpty(request.RejectReason))
            {
                leaveRequest.Description += $" | Red Nedeni: {request.RejectReason}";
            }

            await _leaveRepository.UpdateAsync(leaveRequest);

            return Result<bool>.Success(true, $"İzin talebi {request.NewStatus} olarak güncellendi.");
        }
    }
}