using AutoMapper;
using HRSupport.Application.Common;
using HRSupport.Application.DTOs;
using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetAllLeaveRequestsHandler : IRequestHandler<GetAllLeaveRequestsQuery, Result<IEnumerable<LeaveRequestDto>>>
    {
        private readonly ILeaveRequestRepository _leaveRepository;
        private readonly IMapper _mapper;

        public GetAllLeaveRequestsHandler(ILeaveRequestRepository leaveRepository, IMapper mapper)
        {
            _leaveRepository = leaveRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<LeaveRequestDto>>> Handle(GetAllLeaveRequestsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<LeaveRequest> leaveRequests;

            // Yetkilendirme Kontrolü: 
            // Rol 2 (IKAdmin) veya 4 (Yönetici) ise sadece kendi verilerini görsün.
            // Diğer roller (Admin=1, Çalışan=3, Stajyer=5) tüm listeyi görebilir.
            if (request.Role == "2" || request.Role == "Çalışan" || request.Role == "4" || request.Role == "Stajyer")
            {
                // UI veya API katmanından gelen UserId string'ini int EmployeeId'ye çeviriyoruz.
                if (int.TryParse(request.UserId, out int employeeId))
                {
                    // Repository'e yeni eklediğimiz GetByEmployeeIdAsync metodunu kullanıyoruz.
                    leaveRequests = await _leaveRepository.GetByEmployeeIdAsync(employeeId);
                }
                else
                {
                    // Geçersiz ID durumunda boş liste dönerek güvenliği sağlıyoruz.
                    leaveRequests = new List<LeaveRequest>();
                }
            }
            else
            {
                // Üst roller tüm izin taleplerini çekiyor.
                leaveRequests = await _leaveRepository.GetAllAsync();
            }

            // Entity listesini UI'a gönderilecek DTO listesine eşliyoruz.
            var leaveRequestDtos = _mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);

            return Result<IEnumerable<LeaveRequestDto>>.Success(leaveRequestDtos, "İzin talepleri yetki dahilinde başarıyla getirildi.");
        }
    }
}