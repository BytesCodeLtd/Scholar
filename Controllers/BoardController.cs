using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        private readonly IBoardService _boards;
        private readonly ILogger<BoardController> _logger;

        public BoardController(IBoardService boards, ILogger<BoardController> logger)
        {
            _boards = boards;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogDebug("Loading boards.");
            return View(await _boards.GetBoardsAsync());
        }
    }
}
