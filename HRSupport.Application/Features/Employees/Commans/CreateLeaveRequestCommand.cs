using HRSupport.Application.Common;
using HRSupport.Domain.Enum;
using MediatR;

namespace HRSupport.Application.Features.Commands
    {
        public class CreateLeaveRequestCommand : IRequest<Result<int>>
        {
            public int EmployeeId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public LeaveType Type { get; set; }
            public string Description { get; set; }
        }
    }
}
}
