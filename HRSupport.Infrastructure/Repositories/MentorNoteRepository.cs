using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entities;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Infrastructure.Repositories
{
    public class MentorNoteRepository : IMentorNoteRepository
    {
        private readonly ApplicationDbContext _context;

        public MentorNoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MentorNote>> GetByInternIdAsync(int internId)
        {
            return await _context.MentorNotes
                .Include(x => x.Mentor)
                .Where(x => x.InternId == internId)
                .OrderByDescending(x => x.CreatedTime)
                .ToListAsync();
        }

        public async Task<MentorNote?> GetByIdAsync(int id)
        {
            return await _context.MentorNotes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(MentorNote entity)
        {
            await _context.MentorNotes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
    }
}
