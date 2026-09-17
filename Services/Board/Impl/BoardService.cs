using Microsoft.EntityFrameworkCore;
using Scholar.Models;
using Scholar.Repositories;

namespace Scholar.Services
{
    public class BoardService(IRepository<Board> boards) : IBoardService
    {
        private readonly IRepository<Board> _boards = boards;

        public Task<List<Board>> GetBoardsAsync()
            => _boards.Query().OrderBy(b => b.Id).ToListAsync();
    }
}
