using HRSupport.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRSupport.Infrastructure.Repositories
{
    public class WeeklyReportRepository
    {
        // WeeklyReportRepository.cs içinde
        public async Task<IEnumerable<WeeklyReportRequest>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.WeeklyReportRequests
                .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
                .ToListAsync();
        }
    }
}
