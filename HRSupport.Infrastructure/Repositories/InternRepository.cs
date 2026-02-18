using HRSupport.Application.Interfaces;
using HRSupport.Domain.Entites;
using HRSupport.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRSupport.Infrastructure.Repositories
{
    public class InternRepository : IInternRepository
    {
        private readonly ApplicationDbContext _context;

        public InternRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Intern>> GetAllAsync()
        {
            return await _context.Interns.Include(i => i.Mentor).ToListAsync();
        }

        public async Task<Intern> GetByIdAsync(int id)
        {
            return await _context.Interns.Include(i => i.Mentor).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> AddAsync(Intern entity)
        {
            await _context.Interns.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<Intern> UpdateAsync(Intern intern)
        {
            _context.Interns.Update(intern);
            await _context.SaveChangesAsync();
            return intern;
        }

        public async Task<Intern> DeleteAsync(int id)
        {
            var intern = await _context.Interns.FirstOrDefaultAsync(x => x.Id == id);
            if (intern != null)
            {
                intern.IsDeleted = true;
                _context.Interns.Update(intern);
                await _context.SaveChangesAsync();
                return intern;
            }
            return null;
        }
    }
}