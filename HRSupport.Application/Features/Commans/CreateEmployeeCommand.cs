using HRSupport.Application.Common;
using HRSupport.Domain.Common;
using MediatR;

namespace HRSupport.Application.Features.Commans
{
    public class CreateEmployeeCommand : IRequest<Result<int>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CardID { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public Department Deparment { get; set; }
        public Roles Roles { get; set; }


    }
}
