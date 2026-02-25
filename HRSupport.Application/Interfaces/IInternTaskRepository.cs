using HRSupport.Domain.Entities;

namespace HRSupport.Application.Interfaces
{
    public interface IInternTaskRepository
    {
        Task<IEnumerable<InternTask>> GetByInternIdAsync(int internId);
        Task<InternTask?> GetByIdAsync(int id);
        Task<int> AddAsync(InternTask entity);
        Task UpdateAsync(InternTask entity);
    }
}
