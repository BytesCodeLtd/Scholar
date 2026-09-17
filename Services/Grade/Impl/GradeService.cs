using Microsoft.EntityFrameworkCore;
using Scholar.Models;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class GradeService(IRepository<Grade> grades) : IGradeService
    {
        private readonly IRepository<Grade> _grades = grades;

        public Task<List<Grade>> GetGradesForBoardAsync(int boardId)
            => _grades.Query()
                      .Where(g => g.Subjects.Any(s => s.BoardId == boardId))
                      .OrderBy(g => g.Id)
                      .ToListAsync();
    }
}
