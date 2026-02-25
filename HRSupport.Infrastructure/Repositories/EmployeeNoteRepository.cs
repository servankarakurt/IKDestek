using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Infrastructure.Repositories
{
    public class EmployeeNoteRepository : IEmployeeNoteRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeNoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeNote>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeNotes
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.CreatedTime)
                .ToListAsync();
        }

        public async Task<EmployeeNote?> GetByIdAsync(int id)
        {
            return await _context.EmployeeNotes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(EmployeeNote entity)
        {
            await _context.EmployeeNotes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
    }
}
