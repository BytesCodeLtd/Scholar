using Microsoft.EntityFrameworkCore;
using Scholar.Models;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class SubjectService(IRepository<Subject> subjects) : ISubjectService
    {
        private readonly IRepository<Subject> _subjects = subjects;

        public Task<List<Subject>> GetSubjectsAsync(int boardId, int gradeId)
            => _subjects.Query()
                        .Where(s => s.BoardId == boardId && s.GradeId == gradeId)
                        .OrderBy(s => s.Id)
                        .ToListAsync();
    }
}
