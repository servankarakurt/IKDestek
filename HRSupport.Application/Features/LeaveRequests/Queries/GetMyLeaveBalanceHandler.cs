using HRSupport.Application.Common;
using HRSupport.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.LeaveRequests.Queries
{
    public class GetMyLeaveBalanceHandler : IRequestHandler<GetMyLeaveBalanceQuery, Result<int>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IEmployeeLeaveBalanceRepository _balanceRepository;

        public GetMyLeaveBalanceHandler(ICurrentUserService currentUser, IEmployeeLeaveBalanceRepository balanceRepository)
        {
            _currentUser = currentUser;
            _balanceRepository = balanceRepository;
        }

        public async Task<Result<int>> Handle(GetMyLeaveBalanceQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.UserId.HasValue)
                return Result<int>.Failure("Oturum bulunamadı.");

            var balance = await _balanceRepository.GetByEmployeeIdAsync(_currentUser.UserId.Value);
            return Result<int>.Success(balance?.RemainingAnnualLeaveDays ?? 0, "Kalan izin günü getirildi.");
        }
    }
}
