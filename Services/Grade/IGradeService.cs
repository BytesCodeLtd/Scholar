using Scholar.Models;

namespace Scholar.Services
{
    public interface IGradeService
    {
        Task<List<Grade>> GetGradesForBoardAsync(int boardId);
    }
}
