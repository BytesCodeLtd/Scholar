using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholar.Models;
using Scholar.Repositories;

namespace Scholar.Controllers
{
    [Authorize]
    public class ChapterController : Controller
    {
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IRepository<Institute> _institutes;

        public ChapterController(
            IRepository<Chapter> chapterRepository,
            IRepository<Institute> institutes)
        {
            _chapterRepository = chapterRepository;
            _institutes = institutes;
        }

        public async Task<IActionResult> Index(int subjectId)
        {
            List<Chapter> chapters = await _chapterRepository.Query()
                                                             .Where(x => x.SubjectId == subjectId)
                                                             .Include(c => c.Topics)
                                                             .Include(c => c.Subject)
                                                             .OrderBy(c => c.Number)
                                                             .ToListAsync();

            ViewBag.SubjectId = subjectId;
            ViewBag.GradeId = chapters.FirstOrDefault()?.Subject.GradeId ?? 0;

            if (User.IsInRole(Constants.Roles.SuperAdmin))
            {
                ViewBag.Institutes = await _institutes.Query()
                                                      .AsNoTracking()
                                                      .ToListAsync();
            }

            return View(chapters);
        }

    }
}
