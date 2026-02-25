using HRSupport.Domain.Entities;

namespace HRSupport.Application.Interfaces
{
    public interface IMentorNoteRepository
    {
        Task<IEnumerable<MentorNote>> GetByInternIdAsync(int internId);
        Task<MentorNote?> GetByIdAsync(int id);
        Task<int> AddAsync(MentorNote entity);
    }
}
