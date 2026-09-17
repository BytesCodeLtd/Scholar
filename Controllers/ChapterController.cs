using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scholar.Models;
using Scholar.Services;

namespace Scholar.Controllers
{
    [Authorize]
    public class ChapterController : Controller
    {
        private readonly IChapterService _chapters;
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(IChapterService chapters, ILogger<ChapterController> logger)
        {
            _chapters = chapters;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int subjectId)
        {
            _logger.LogDebug("Loading chapters for subject {SubjectId}.", subjectId);
            List<Chapter> chapters = await _chapters.GetChaptersAsync(subjectId);

            ViewBag.SubjectId = subjectId;
            ViewBag.GradeId = chapters.FirstOrDefault()?.Subject.GradeId ?? 0;

            if (_chapters.IsSuperAdmin)
            {
                ViewBag.Institutes = await _chapters.GetInstitutesAsync();
            }

            return View(chapters);
        }
    }
}
