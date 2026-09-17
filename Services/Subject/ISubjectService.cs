using Scholar.Models;

namespace Scholar.Services
{
    public interface ISubjectService
    {
        Task<List<Subject>> GetSubjectsAsync(int boardId, int gradeId);
    }
}
