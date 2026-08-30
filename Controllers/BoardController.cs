using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholar.Models;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        private readonly IRepository<Board> _boards;

        public BoardController(IRepository<Board> boards)
        {
            _boards = boards;
        }

        public async Task<IActionResult> Index()
        {
            List<Board>? boards = await _boards.Query()
                                               .OrderBy(b => b.Id)
                                               .ToListAsync();

            return View(boards);
        }
    }
}
