using HRSupport.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRSupport.Application.Interfaces
{
    // WeeklyReportRepository.cs içinde
    public interface IWeeklyReportRepository
    {
        Task<IEnumerable<WeeklyReportRequest>> GetByEmployeeIdAsync(int employeeId);
    }
}
