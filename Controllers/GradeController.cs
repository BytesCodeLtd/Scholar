using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        private readonly IGradeService _grades;
        private readonly ILogger<GradeController> _logger;

        public GradeController(IGradeService grades, ILogger<GradeController> logger)
        {
            _grades = grades;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int boardId)
        {
            _logger.LogDebug("Loading grades for board {BoardId}.", boardId);
            ViewBag.BoardId = boardId;
            return View(await _grades.GetGradesForBoardAsync(boardId));
        }
    }
}
