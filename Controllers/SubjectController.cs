using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjects;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(ISubjectService subjects, ILogger<SubjectController> logger)
        {
            _subjects = subjects;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int boardId, int gradeId)
        {
            _logger.LogDebug("Loading subjects for board {BoardId}, grade {GradeId}.", boardId, gradeId);
            ViewBag.BoardId = boardId;
            ViewBag.GradeId = gradeId;
            return View(await _subjects.GetSubjectsAsync(boardId, gradeId));
        }
    }
}
