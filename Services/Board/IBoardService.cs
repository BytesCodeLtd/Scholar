using Scholar.Models;

namespace Scholar.Services
{
    public interface IBoardService
    {
        Task<List<Board>> GetBoardsAsync();
    }
}
