using HRSupport.Application.Common;
using HRSupport.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRSupport.Application.Features.Employees.Queries
{
    public class GetAllEmployeesQuery: IRequest<Result<IEnumerable<Employee>>>
    {
        
    }
}
