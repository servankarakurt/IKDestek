using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Infrastructure.Repositories
{
    public class InternTaskRepository : IInternTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public InternTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InternTask>> GetByInternIdAsync(int internId)
        {
            return await _context.InternTasks
                .Where(x => x.InternId == internId)
                .OrderByDescending(x => x.CreatedTime)
                .ToListAsync();
        }

        public async Task<InternTask?> GetByIdAsync(int id)
        {
            return await _context.InternTasks.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(InternTask entity)
        {
            await _context.InternTasks.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(InternTask entity)
        {
            _context.InternTasks.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
